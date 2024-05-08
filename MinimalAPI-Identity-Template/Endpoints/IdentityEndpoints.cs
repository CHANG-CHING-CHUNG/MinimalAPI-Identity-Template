using System.Collections;
using System.Web;
using Microsoft.AspNetCore.Identity;
using MinimalApiIdentityTemplate.ExampleModels;
using MinimalApiIdentityTemplate.Models;
using MinimalApiIdentityTemplate.Services;

namespace MinimalApiIdentityTemplate.Endpoints;

public static class IdentityEndpoints
{
    private const string Success = "Success";
    private const string Failure = "Failure";

    private static IResult GetBadRequestResponse(object errors)
    {
        return TypedResults.BadRequest(new
        {
            Status = Failure,
            Errors = errors
        });
    }

    private static IResult GetOkResponse(object data)
    {
        if (data is IList && data.GetType().IsGenericType)
            return Results.Ok(new
            {
                Status = Success,
                Data = data
            });

        return Results.Ok(new
        {
            Status = Success,
            Data = new List<object> { data }
        });
    }

    public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group)
    {
        group.MapPost("/register",
                async (ApplicationUser userModel, UserManager<AppUser> userManager) =>
                {
                    var user = new AppUser
                        { UserName = userModel.Email, Email = userModel.Email, NickName = userModel.NickName };
                    var result = await userManager.CreateAsync(user, userModel.Password);
                    if (!result.Succeeded) return GetBadRequestResponse(result.Errors);

                    await userManager.AddToRoleAsync(user, "User");
                    var roles = await userManager.GetRolesAsync(user);
                    var userInfo = new
                    {
                        user.Id,
                        UserName = user.Email,
                        user.Email,
                        user.NickName,
                        user.EmailConfirmed,
                        Roles = roles
                    };
                    return GetOkResponse(userInfo);
                }).AddEndpointFilter(async (efiContext, next) =>
            {
                var userModelParam = efiContext.GetArgument<ApplicationUser>(0);
                if (string.IsNullOrEmpty(userModelParam.UserName) || string.IsNullOrEmpty(userModelParam.Email) ||
                    string.IsNullOrEmpty(userModelParam.Password))
                    return GetBadRequestResponse(new List<object>
                    {
                        new { Code = "UserDataEmpty", Description = "Username Email or password cannot be empty" }
                    });

                return await next(efiContext);
            }).WithTags("Users").Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapGet("/user-manage/users", (UserManager<AppUser> userManager) =>
            {
                var users = userManager.Users.ToList().Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.NickName,
                    user.EmailConfirmed,
                    Roles = userManager.GetRolesAsync(user).Result.ToArray()
                }).ToList();
                return GetOkResponse(users);
            }).RequireAuthorization("Admin").WithTags("Users")
            .Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapGet("/user-manage/user", async (HttpContext httpContext, UserManager<AppUser> userManager) =>
            {
                var user = await userManager.GetUserAsync(httpContext.User);

                if (user == null) return Results.NotFound();

                return GetOkResponse(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.EmailConfirmed,
                    user.NickName,
                    Roles = userManager.GetRolesAsync(user).Result.ToArray()
                });
            }).RequireAuthorization().WithTags("Users").Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapGet("/user-manage/users/{id}", async (string id, UserManager<AppUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null) return Results.NotFound();

                return GetOkResponse(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.EmailConfirmed,
                    user.NickName,
                    Roles = userManager.GetRolesAsync(user).Result.ToArray()
                });
            }).RequireAuthorization("Admin").WithTags("Users").Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapPatch("/user-manage/users/{id}",
                async (string id, HttpContext httpContext, UpdateUserModel? updateUserModel,
                    UserManager<AppUser> userManager) =>
                {
                    var user = await userManager.FindByIdAsync(id);
                    var currentUser = await userManager.GetUserAsync(httpContext.User);
                    var isCurrentUserAdmin = currentUser != null &&
                                             userManager.GetRolesAsync(currentUser).Result.Contains("Admin");
                    if (user == null) return Results.NotFound();

                    if (user.Id != currentUser?.Id && !isCurrentUserAdmin)
                        return GetBadRequestResponse(new List<object>
                        {
                            new { Code = "UpdateCurrentUserOnly", Description = "Cannot update another user's profile" }
                        });

                    if (updateUserModel == null || string.IsNullOrEmpty(updateUserModel.NickName))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "PostBodyCannotBeEmpty",
                                Description = "Post body must contains data to update the user"
                            }
                        });

                    user.NickName = updateUserModel.NickName;
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded) return GetBadRequestResponse(result.Errors);

                    return GetOkResponse(new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.EmailConfirmed,
                        user.NickName,
                        Roles = userManager.GetRolesAsync(user).Result.ToArray()
                    });
                }).RequireAuthorization().WithTags("Users").WithOpenApi(option =>
            {
                option.Responses["400"].Description = "Invalid Operation";
                return option;
            }).Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapDelete("/user-manage/users/{id}",
            async (string id, HttpContext httpContext, UserManager<AppUser> userManager) =>
            {
                var user = await userManager.FindByIdAsync(id);
                var currentUser = await userManager.GetUserAsync(httpContext.User);
                if (user == null) return Results.NotFound();

                if (user.Id == currentUser?.Id)
                    return GetBadRequestResponse(new List<object>
                        { new { Code = "DeleteSelfNotAllowed", Description = "Cannot delete yourself" } });

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded) return GetBadRequestResponse(result.Errors);

                return Results.Ok();
            }).RequireAuthorization("Admin").WithTags("Users");

        return group;
    }

    public static RouteGroupBuilder MapRoleApi(this RouteGroupBuilder group)
    {
        group.MapPut("/role-manage/users/{id}",
                async (string id, HttpContext httpContext, UserManager<AppUser> userManager,
                    RoleManager<IdentityRole> roleManager) =>
                {
                    var user = await userManager.FindByIdAsync(id);
                    var currentUser = await userManager.GetUserAsync(httpContext.User);
                    if (user == null) return Results.NotFound();

                    if (user.Id == currentUser?.Id)
                        return GetBadRequestResponse(new List<object>
                            { new { Code = "EditSelfNotAllowed", Description = "Cannot edit the role of yourself" } });

                    if (!httpContext.Request.HasJsonContentType())
                        return GetBadRequestResponse(new List<object>
                            { new { Code = "ContentTypeNotJson", Description = "Content type must be Json Format" } });

                    var roleModel = await httpContext.Request.ReadFromJsonAsync<RoleModel>();
                    if (roleModel == null || string.IsNullOrEmpty(roleModel.Name))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "PostBodyCannotBeEmpty",
                                Description = "Post body must contains data to update the user"
                            }
                        });

                    var oldRoles = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRolesAsync(user, oldRoles);
                    var hasRole = await roleManager.RoleExistsAsync(roleModel.Name);
                    if (!hasRole)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "RoleNotExist",
                                Description = "The role is not exist"
                            }
                        });

                    var result = await userManager.AddToRoleAsync(user, roleModel.Name);
                    var newRoles = await userManager.GetRolesAsync(user);
                    return !result.Succeeded
                        ? GetBadRequestResponse(result.Errors)
                        : GetOkResponse(new
                        {
                            user.Id,
                            UserName = user.Email,
                            user.Email,
                            user.NickName,
                            user.EmailConfirmed,
                            Roles = newRoles
                        });
                }).RequireAuthorization("Admin").WithTags("Roles").Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapPost("/role-manage/roles",
                async (HttpContext httpContext, RoleManager<IdentityRole> roleManager) =>
                {
                    var addRoleModel = await httpContext.Request.ReadFromJsonAsync<AddRoleModel>();
                    if (addRoleModel == null || string.IsNullOrEmpty(addRoleModel.Name))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "PostBodyCannotBeEmpty",
                                Description = "Post body must contains data to update the role"
                            }
                        });

                    var newRole = new IdentityRole { Name = addRoleModel.Name };
                    var result = await roleManager.CreateAsync(newRole);
                    if (!result.Succeeded) return GetBadRequestResponse(result.Errors);


                    return GetOkResponse(new { Message = $"Role {newRole.Name} created successfully" });
                }).RequireAuthorization("Admin").WithTags("Roles").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapDelete("/role-manage/roles/{id}", async (string id, RoleManager<IdentityRole> roleManager) =>
            {
                var roleToDelete = await roleManager.FindByIdAsync(id);
                if (roleToDelete == null) return Results.NotFound();

                if (roleToDelete.Name is "Admin")
                    return GetBadRequestResponse(new List<object>
                    {
                        new
                        {
                            Code = "RoleAdminDeletionNotAllowed",
                            Description = "Role 'Admin' cannot be deleted"
                        }
                    });

                var result = await roleManager.DeleteAsync(roleToDelete);
                if (!result.Succeeded) return GetBadRequestResponse(result.Errors);

                return GetOkResponse(new { Message = "Role Deletion succeeded" });
            }).RequireAuthorization("Admin").WithTags("Roles").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapPatch("role-manage/roles/{id}",
                async (string id, RoleModel roleModel, RoleManager<IdentityRole> roleManager) =>
                {
                    var roleToUpdate = await roleManager.FindByIdAsync(id);
                    if (roleToUpdate == null)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "RoleNotFound",
                                Description = "Role Not Found"
                            }
                        });

                    if (roleToUpdate.Name is "Admin")
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "RoleAdminModificationNotAllowed",
                                Description = "Role 'Admin' cannot be modified"
                            }
                        });

                    if (string.IsNullOrEmpty(roleModel.Name))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "EmptyRoleNameNotAllowed",
                                Description = "Role name cannot be empty"
                            }
                        });

                    roleToUpdate.Name = roleModel.Name;
                    var result = await roleManager.UpdateAsync(roleToUpdate);
                    if (!result.Succeeded) return GetBadRequestResponse(result.Errors);
                    return GetOkResponse(new
                    {
                        Message = "Role update succeeded"
                    });
                }).RequireAuthorization("Admin").WithTags("Roles").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapGet("/role-manage/roles", (RoleManager<IdentityRole> roleManager) =>
            {
                var roleList = roleManager.Roles.ToList().Select(role =>
                {
                    var mappedRole = new { role.Id, role.Name };
                    return mappedRole;
                }).ToList();
                return GetOkResponse(roleList);
            }).RequireAuthorization("Admin").WithTags("Roles").Produces<ResponseWrapper<RoleExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        return group;
    }

    public static RouteGroupBuilder MapLoginApi(this RouteGroupBuilder group)
    {
        group.MapPost("/login",
                async (LoginRequest request, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) =>
                {
                    var user = await userManager.FindByEmailAsync(request.Email);
                    if (user == null)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "InvalidEmailOrPassword",
                                Description = "Invalid email or password"
                            }
                        });

                    var result =
                        await signInManager.PasswordSignInAsync(user, request.Password, true, false);
                    if (!result.Succeeded)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "InvalidEmailOrPassword",
                                Description = "Invalid email or password"
                            }
                        });

                    var roles = await userManager.GetRolesAsync(user);
                    var userInfo = new
                    {
                        user.Id,
                        UserName = user.Email,
                        user.Email,
                        user.NickName,
                        user.EmailConfirmed,
                        Roles = roles
                    };
                    return GetOkResponse(userInfo);
                }).WithTags("Login").Produces<ResponseWrapper<UserExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapPost("/logout", async (SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return GetOkResponse(new { Message = "Logout succeeded" });
            }).RequireAuthorization().WithTags("Login").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        return group;
    }

    public static RouteGroupBuilder MapForgetPasswordApi(this RouteGroupBuilder group)
    {
        group.MapPost("/user-manage/forgot-password",
                async (ForgotPasswordModel forgotPasswordModel, UserManager<AppUser> userManager,
                    IEmailService emailService) =>
                {
                    if (string.IsNullOrEmpty(forgotPasswordModel.Email))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "PostBodyCannotBeEmpty",
                                Description = "Post body must contains data"
                            }
                        });

                    var user = await userManager.FindByEmailAsync(forgotPasswordModel.Email);
                    if (user == null)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "UserIsNotExisting",
                                Description = "User doesn't exist"
                            }
                        });

                    var token = HttpUtility.UrlEncode(await userManager.GeneratePasswordResetTokenAsync(user));
                    var clientHttpsUrl = ConfigurationHelper.config.GetSection("clientUrls:https").Value!;
                    var resetPasswordLink = $"{clientHttpsUrl}/forgot-password?userId={user.Id}&token={token}";
                    var fromEmail = ConfigurationHelper.config.GetSection("EmailUsername").Value!;
                    var emailDto = new EmailDto
                    {
                        From = fromEmail, To = forgotPasswordModel.Email, Subject = "重設密碼連結",
                        Body = $"<a href=\"{resetPasswordLink}\">重設密碼連結</a>"
                    };
                    await emailService.SendEmailAsync(emailDto);
                    return GetOkResponse(new { Message = $"Email has been sent to {emailDto.To}" });
                }).WithTags("Password").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapPost("/user-manage/confirm-reset-password",
                async (ConfirmResetPasswordModel confirmResetPasswordModel, UserManager<AppUser> userManager) =>
                {
                    if (string.IsNullOrEmpty(confirmResetPasswordModel.UserId) ||
                        string.IsNullOrEmpty(confirmResetPasswordModel.Token))
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "PostBodyCannotBeEmpty",
                                Description = "Post body must contains data"
                            }
                        });

                    var user = await userManager.FindByIdAsync(confirmResetPasswordModel.UserId);
                    if (user == null)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "UserIsNotExisting",
                                Description = "User doesn't exist"
                            }
                        });

                    var result = await userManager.ResetPasswordAsync(user, confirmResetPasswordModel.Token,
                        confirmResetPasswordModel.NewPassword);
                    if (!result.Succeeded) return GetBadRequestResponse(result.Errors);
                    return GetOkResponse(new { Message = "Password reset succeeded" });
                }).WithTags("Password").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);
        return group;
    }

    public static RouteGroupBuilder MapEmailLoginApi(this RouteGroupBuilder group)
    {
        group.MapPost("/send-login-email",
                async (LoginEmailModel emailModel, UserManager<AppUser> userManager, IEmailService emailService) =>
                {
                    var user = await userManager.FindByEmailAsync(emailModel.Email);
                    if (user == null)
                        return GetBadRequestResponse(new List<object>
                        {
                            new
                            {
                                Code = "UserIsNotExisting",
                                Description = "User doesn't exist"
                            }
                        });

                    var loginToken = HttpUtility.UrlEncode(await userManager.GenerateEmailConfirmationTokenAsync(user));
                    var serverHttpsUrl = ConfigurationHelper.config.GetSection("serverUrls:https").Value!;
                    var loginLink = $"{serverHttpsUrl}/api/identity/confirm-login?userId={user.Id}&token={loginToken}";
                    var fromEmail = ConfigurationHelper.config.GetSection("EmailUsername").Value!;
                    var emailDto = new EmailDto
                    {
                        From = fromEmail, To = emailModel.Email, Subject = "Email 登入連結",
                        Body = $"<a href=\"{loginLink}\">請點擊登入</a>"
                    };
                    await emailService.SendEmailAsync(emailDto);
                    return GetOkResponse(new { Message = $"Login Email has been sent to {emailDto.To}" });
                }).WithTags("Email Login").Produces<ResponseWrapper<MessageExample[]>>()
            .Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

        group.MapGet("/confirm-login", async (string userId, string token, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return GetBadRequestResponse(new List<object>
                {
                    new
                    {
                        Code = "UserIsNotExisting",
                        Description = "User doesn't exist"
                    }
                });

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return GetBadRequestResponse(result.Errors);

            var clientHttpsUrl = ConfigurationHelper.config.GetSection("clientUrls:https").Value!;
            await signInManager.SignInAsync(user, false);
            return Results.Redirect($"{clientHttpsUrl}/current-user");
        }).WithTags("Email Login").Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);
        return group;
    }
}