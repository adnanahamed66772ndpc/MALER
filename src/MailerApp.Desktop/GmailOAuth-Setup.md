# Gmail OAuth setup

The **"Missing required parameter: client_id"** error means the app is not configured with Google OAuth credentials. Do this once:

## 1. Create OAuth credentials in Google Cloud

1. Go to [Google Cloud Console](https://console.cloud.google.com/apis/credentials).
2. Select or create a project.
3. Click **Create credentials** → **OAuth client ID**.
4. If asked, configure the **OAuth consent screen** (User type: External, add your email as test user).
5. For **Application type** choose **Desktop app** (or **Web application** if you prefer).
6. Name it (e.g. "Mailer App") and click **Create**.
7. Copy the **Client ID** and **Client secret**.

## 2. Set redirect URI (if using Web application)

- If you chose **Web application**, add this **Authorized redirect URI**:  
  `http://localhost:8080/`
- For **Desktop app**, redirect is often not required for the same flow; keep **RedirectUri** in config as `http://localhost:8080/` to match the code.

## 3. Put credentials in appsettings.json

Edit `src/MailerApp.Desktop\appsettings.json` (and the same file in the output folder if you run from bin):

```json
{
  "GmailOAuth": {
    "ClientId": "YOUR_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "RedirectUri": "http://localhost:8080/"
  }
}
```

Replace `YOUR_CLIENT_ID` and `YOUR_CLIENT_SECRET` with the values from step 1.

## 4. Restart the app

Close and run the app again. Click **Add Gmail (OAuth)**; the browser should open Google sign-in with your app name and no more "client_id" error.

---

**Alternative: use Gmail SMTP instead**

You can skip OAuth and add the account with **Add Gmail (SMTP)** using your Gmail address and an [App Password](https://myaccount.google.com/apppasswords) (2-Step Verification must be on). No Google Cloud project needed.
