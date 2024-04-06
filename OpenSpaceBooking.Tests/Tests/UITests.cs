using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenSpaceBooking.Tests.Tests;

public class UITests : IDisposable
{
    private readonly IWebDriver _driver;
    public UITests()
    {
        _driver = new ChromeDriver();
        _driver.Navigate().GoToUrl("https://openspaces.azurewebsites.net/OpenSpace");
    }

    [Fact]
    public void UI_test_CheckTitlet()
    {
        Assert.NotEmpty(_driver.Title);
        Assert.NotNull(_driver.Title);
        Assert.Equal("Open Spaces - OpenSpaceBooking.Presentation", _driver.Title);
    }

    [Fact]
    public void UI_test_CheckPanelTitle()
    {
        var panelText = _driver.FindElement(By.CssSelector("a.text-white.text-2xl[href='/']"));

        Assert.NotNull(panelText);
        Assert.NotNull(panelText.Text);
        Assert.NotEmpty(panelText.Text);
        Assert.Equal("OpenSpaceBooking", panelText.Text);
    }

    [Fact]
    public void UI_test_CheckRegisterButton()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));

        Assert.NotNull(registerButton);
        Assert.NotNull(registerButton.Text);
        Assert.NotEmpty(registerButton.Text);
        Assert.Equal("Register", registerButton.Text);
    }
    

    [Fact]
    public void UI_test_CheckRegisterEmailField()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var emailfield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='Email']"));

        Assert.NotNull(emailfield);
        Assert.NotNull(emailfield.Text);
        Assert.NotEmpty(emailfield.Text);
        Assert.Equal("Email", emailfield.Text);
    }

    [Fact]
    public void UI_test_CheckRegisterPasswordField()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var passwordfield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='Password']"));

        Assert.NotNull(passwordfield);
        Assert.NotNull(passwordfield.Text);
        Assert.NotEmpty(passwordfield.Text);
        Assert.Equal("Password", passwordfield.Text);
    }


    [Fact]
    public void UI_test_CheckRegisterConfirmPasswordField()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var conpasswordfield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='ConfirmPassword']"));

        Assert.NotNull(conpasswordfield);
        Assert.NotNull(conpasswordfield.Text);
        Assert.NotEmpty(conpasswordfield.Text);
        Assert.Equal("ConfirmPassword", conpasswordfield.Text);
    }

    [Fact]
    public void UI_test_CheckRegisterFirstNameField()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var FirstNamefield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='FirstName']"));

        Assert.NotNull(FirstNamefield);
        Assert.NotNull(FirstNamefield.Text);
        Assert.NotEmpty(FirstNamefield.Text);
        Assert.Equal("FirstName", FirstNamefield.Text);
    }

    [Fact]
    public void UI_test_CheckRegisterLastNameField()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var LastNamefield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='LastName']"));

        Assert.NotNull(LastNamefield);
        Assert.NotNull(LastNamefield.Text);
        Assert.NotEmpty(LastNamefield.Text);
        Assert.Equal("LastName", LastNamefield.Text);
    }


    [Fact]
    public void UI_test_CheckSignUpButtonText()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var signupButton = _driver.FindElement(By.XPath("//button[contains(text(),'Sign Up')]"));

        Assert.NotNull(signupButton);
        Assert.NotNull(signupButton.Text);
        Assert.NotEmpty(signupButton.Text);
        Assert.Equal("Sign Up", signupButton.Text);
    }


    [Fact]
    public void UI_test_CheckRegisterInputFields()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();

        var NameElement = _driver.FindElement(By.Id("Email"));
        Assert.NotNull(NameElement);

        var PasswordElement = _driver.FindElement(By.Id("Password"));
        Assert.NotNull(PasswordElement);

        var ConfirmPasswordElement = _driver.FindElement(By.Id("ConfirmPassword"));
        Assert.NotNull(ConfirmPasswordElement);

        var FirstNameElement = _driver.FindElement(By.Id("FirstName"));
        Assert.NotNull(FirstNameElement);

        var LastNameElement = _driver.FindElement(By.Id("LastName"));
        Assert.NotNull(LastNameElement);
    }


    [Fact]
    public void UI_test_CheckRegisterUser()
    {
        var registerButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Register']"));
        registerButton.Click();
        string email = $"solomia1212+{DateTime.Now:yyyyMMddHHmmssfff}@gmail.com";
        _driver.FindElement(By.Id("Email")).SendKeys(email);
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-");
        _driver.FindElement(By.Id("ConfirmPassword")).SendKeys("Qwerty1-");
        _driver.FindElement(By.Id("FirstName")).SendKeys("name");
        _driver.FindElement(By.Id("LastName")).SendKeys("surname");


        var signupButton = _driver.FindElement(By.XPath("//button[contains(text(),'Sign Up')]"));
        signupButton.Click();


        var notificationDiv = _driver.FindElement(By.CssSelector("div.pt-64.text-3xl.text-black.text-center.font-semibold"));
        // Assert the presence of the div
        Assert.NotNull(notificationDiv);

        // Assert the text content of the div
        Assert.Equal("Please check your email for the verification link.", notificationDiv.Text);
    }




    [Fact]
    public void UI_test_CheckLoginButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));

        Assert.NotNull(loginButton);
        Assert.NotNull(loginButton.Text);
        Assert.NotEmpty(loginButton.Text);
        Assert.Equal("Login", loginButton.Text);
    }


    [Fact]
    public void UI_test_CheckLoginEmailField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        var Emailfield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='Email']"));

        Assert.NotNull(Emailfield);
        Assert.NotNull(Emailfield.Text);
        Assert.NotEmpty(Emailfield.Text);
        Assert.Equal("Email", Emailfield.Text);
    }


    [Fact]
    public void UI_test_CheckLoginPasswordField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        var Passwordfield = _driver.FindElement(By.CssSelector("label.block.text-gray-700.text-sm.font-bold.mb-2[for='Password']"));

        Assert.NotNull(Passwordfield);
        Assert.NotNull(Passwordfield.Text);
        Assert.NotEmpty(Passwordfield.Text);
        Assert.Equal("Password", Passwordfield.Text);
    }



    [Fact]
    public void UI_test_CheckForgotPasswordField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        var forgotPasswordLink = _driver.FindElement(By.CssSelector("a[href='/Auth/ForgotPassword']"));

        Assert.NotNull(forgotPasswordLink);
        Assert.NotNull(forgotPasswordLink.Text);
        Assert.NotEmpty(forgotPasswordLink.Text);
        Assert.Equal("Forgot Password?", forgotPasswordLink.Text);
    }


    [Fact]
    public void UI_test_CheckSignInButtonText()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

        Assert.NotNull(signInButton);
        Assert.NotNull(signInButton.Text);
        Assert.NotEmpty(signInButton.Text);
        Assert.Equal("Sign In", signInButton.Text);
    }


    [Fact]
    public void UI_test_CheckLoginInputFields()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        var NameElement = _driver.FindElement(By.Id("Email"));
        Assert.NotNull(NameElement);

        var PasswordElement = _driver.FindElement(By.Id("Password"));
        Assert.NotNull(PasswordElement);

    }


    [Fact]
    public void UI_test_CheckLoginAsAdmin()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        // Assert the presence of the div
        Assert.NotNull(addOpenSpaceLink);

        Assert.Equal("Add Open Space", addOpenSpaceLink.Text);
    }


    [Fact]
    public void UI_test_CheckLogOutButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var logoutButton = _driver.FindElement(By.CssSelector("button[b-e4idrsf6n0='']"));
        Assert.NotNull(logoutButton);

        Assert.Equal("Logout", logoutButton.Text);
        logoutButton.Click();

        var addOpenSpaceLink = _driver.FindElements(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        // Assert absence of button
        Assert.Empty(addOpenSpaceLink);
    }


    [Fact]
    public void UI_test_CheckMyProfileButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        // Find the "My profile" link
        var myProfileLink = _driver.FindElement(By.CssSelector("a[href='/Reservation']"));
        // Assert the presence of the div
        Assert.NotNull(myProfileLink);

        Assert.Equal("My profile", myProfileLink.Text);
    }



    [Fact]
    public void UI_test_CheckEditProfileButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();

        var myProfileLink = _driver.FindElement(By.CssSelector("a[href='/Reservation']"));
        myProfileLink.Click();


        // Find the "Edit profile" link
        var editProfileLink = _driver.FindElement(By.CssSelector("a[href='/Auth/EditProfile']"));
        // Assert the presence of the div
        Assert.NotNull(editProfileLink);

        Assert.Equal("Edit profile", editProfileLink.Text);
    }


    [Fact]
    public void UI_test_CheckChangePasswordButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();

        var myProfileLink = _driver.FindElement(By.CssSelector("a[href='/Reservation']"));
        myProfileLink.Click();


        // Find the "Change password" link
        var ChangePasswordLink = _driver.FindElement(By.CssSelector("a[href='/Auth/ChangePassword']"));
        // Assert the presence of the div
        Assert.NotNull(ChangePasswordLink);

        Assert.Equal("Change password", ChangePasswordLink.Text);
    }


    [Fact]
    public void UI_test_CheckDeleteAcountButton()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();

        var myProfileLink = _driver.FindElement(By.CssSelector("a[href='/Reservation']"));
        myProfileLink.Click();


        // Find the "Delete account" link
        var deleteAc = _driver.FindElement(By.CssSelector("a[href='/Auth/DeleteProfile']"));
        // Assert the presence of the div
        Assert.NotNull(deleteAc);

        Assert.Equal("Delete account", deleteAc.Text);
    }


    [Fact]
    public void UI_test_CheckAddOpenSpaceAddressField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        addOpenSpaceLink.Click();

        var addressLabel = _driver.FindElement(By.CssSelector("label[for='Address']"));
        // Assert the presence of the label
        Assert.NotNull(addressLabel);

        // Assert the text content of the label
        Assert.Equal("Address", addressLabel.Text);

    }



    [Fact]
    public void UI_test_CheckAddOpenSpacePricePerHourField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        addOpenSpaceLink.Click();

        var PricePerHourLabel = _driver.FindElement(By.CssSelector("label[for='PricePerHour']"));
        // Assert the presence of the label
        Assert.NotNull(PricePerHourLabel);

        // Assert the text content of the label
        Assert.Equal("PricePerHour", PricePerHourLabel.Text);

    }


    [Fact]
    public void UI_test_CheckAddOpenSpaceImageUrlField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        addOpenSpaceLink.Click();

        var ImageUrlLabel = _driver.FindElement(By.CssSelector("label[for='ImageUrl']"));
        // Assert the presence of the label
        Assert.NotNull(ImageUrlLabel);

        // Assert the text content of the label
        Assert.Equal("ImageUrl", ImageUrlLabel.Text);

    }


    [Fact]
    public void UI_test_CheckAddOpenSpaceTitleField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        addOpenSpaceLink.Click();

        var TitleLabel = _driver.FindElement(By.CssSelector("label[for='Title']"));
        // Assert the presence of the label
        Assert.NotNull(TitleLabel);

        // Assert the text content of the label
        Assert.Equal("Title", TitleLabel.Text);

    }


    [Fact]
    public void UI_test_CheckAddOpenSpaceDescriptionField()
    {
        var loginButton = _driver.FindElement(By.CssSelector("a.text-white.text-md.text-right.font-light[href='/Auth/Login']"));
        loginButton.Click();

        _driver.FindElement(By.Id("Email")).SendKeys("admin@gmail.com"); ;
        _driver.FindElement(By.Id("Password")).SendKeys("Qwerty1-"); ;

        var signInButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        signInButton.Click();


        var addOpenSpaceLink = _driver.FindElement(By.CssSelector("a[href='/OpenSpace/PostOpenSpace']"));
        addOpenSpaceLink.Click();

        var DescriptionLabel = _driver.FindElement(By.CssSelector("label[for='Description']"));
        // Assert the presence of the label
        Assert.NotNull(DescriptionLabel);

        // Assert the text content of the label
        Assert.Equal("Description", DescriptionLabel.Text);

    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}

