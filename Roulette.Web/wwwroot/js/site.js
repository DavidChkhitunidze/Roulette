const apiBasePath = "https://localhost:44317/api";

// Handle error messages from api
function errors(jsonResult) {
    if (jsonResult === undefined) {
        return;
    }

    if (!jsonResult.success && jsonResult.errorMessages !== undefined && jsonResult.errorMessages !== null) {
        $(".text-danger").text(jsonResult.errorMessages.join("\r\n"));
    }
}

// Save authentication data and user info in cookies
function saveCookies(jsonResult) {
    if (jsonResult === undefined) {
        return;
    }

    var expirationTime = 1 / 288;
    var id = "id";
    var userName = "userName";
    var firstName = "firstName";
    var lastName = "lastName";
    var userBalance = "userBalance";
    var jwtToken = "jwtToken";
    var refreshToken = "refreshToken";

    Cookies.remove(id, { path: "/" });
    Cookies.remove(userName, { path: "/" });
    Cookies.remove(firstName, { path: "/" });
    Cookies.remove(lastName, { path: "/" });
    Cookies.remove(userBalance, { path: "/" });
    Cookies.remove(jwtToken, { path: "/" });
    Cookies.remove(refreshToken, { path: "/" });

    Cookies.set(id, jsonResult.id, { expires: expirationTime, path: "/" });
    Cookies.set(userName, jsonResult.userName, { expires: expirationTime, path: "/" });
    Cookies.set(firstName, jsonResult.firstName, { expires: expirationTime, path: "/" });
    Cookies.set(lastName, jsonResult.lastName, { expires: expirationTime, path: "/" });
    Cookies.set(userBalance, jsonResult.userBalance, { expires: expirationTime, path: "/" });
    Cookies.set(jwtToken, jsonResult.jwtToken, { expires: expirationTime, path: "/" });
    Cookies.set(refreshToken, jsonResult.refreshToken, { expires: expirationTime, path: "/" });
}

// Refreshing token for every action by authenticated user. 
// access_token expires in every 5 minutes and inactive user will be loged out automaticaly
// With refresh token user refreshes access_token on every server side request and access_token expiration time resets with 5 minutes
function refreshToken() {
    var token = Cookies.get("jwtToken");
    var refreshToken = Cookies.get("refreshToken");

    var data = { "token": token, "refreshToken": refreshToken };
    //data = JSON.stringify(data);

    $.ajax({
        type: "PUT",
        url: apiBasePath + "/account/token?token=" + token + "&refreshtoken=" + refreshToken,
        //contentType: "application/json; charset-utf-8",
        dataType: "json",
        //data: data,
        //data: "token=" + token + "&refreshToken=" + refreshToken,
        success: function (result) {
            if (result.success) {
                saveCookies(result.model);
            }
        },
        error: function (result) {
            errors(result.responseJSON);
        }
    });
}

$(document).ready(function () {
    $("#account-login").submit(function (e) {
        e.preventDefault();

        var serializedForm = $(this).serialize();

        $.ajax({
            type: "POST",
            url: apiBasePath + "/account/login",
            data: serializedForm ,
            success: function (result) {
                if (result.success) {
                    saveCookies(result.model);
                    window.location.href = "/roulette/game";
                }
            },
            error: function (result) {
                errors(result.responseJSON);
            }
        });
    });

    $("#account-register").submit(function (e) {
        e.preventDefault();

        var serializedForm = $(this).serialize();

        $.ajax({
            type: "POST",
            url: apiBasePath + "/account/register",
            data: serializedForm,
            success: function (result) {
                if (result.success) {
                    saveCookies(result.model);
                    window.location.href = "/roulette/game";
                }
            },
            error: function (result) {
                errors(result.responseJSON);
            }
        });
    });

    $("#make-bet").submit(function (e) {
        e.preventDefault();

        var serializedForm = $(this).serialize();
        var token = Cookies.get("jwtToken");

        $.ajax({
            type: "POST",
            url: apiBasePath + "/roulette/bet",
            beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', 'Bearer ' + token); },
            data: serializedForm,
            success: function (result) {
                if (result.success) {
                    $(".text-success").text("Bet Amount: " + result.model.betAmount + " / " + "Won Amount: " + result.model.wonAmount);
                }
                refreshToken();
            },
            error: function (result) {
                if (result.status !== undefined && result.status !== null && result.status === 401) {
                    $(".text-danger").text("You have been inactive. Please log in again.");
                }
                else {
                    errors(result.responseJSON);
                    refreshToken();
                }
            }
        });
    });
});