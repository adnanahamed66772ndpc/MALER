# Git push না হলে কী করবেন

## ⭐ সবচেয়ে কমন কারণ: GitHub এ লগইন (PAT লাগে)

**পাসওয়ার্ড দিয়ে আর পুশ হয় না।** GitHub এ এখন **Personal Access Token (PAT)** দিতে হয়।

### ধাপ ১: GitHub এ Token বানান

1. GitHub এ লগইন করে যান: [https://github.com/settings/tokens](https://github.com/settings/tokens)
2. **"Generate new token"** → **"Generate new token (classic)"** চাপুন।
3. Note লিখুন: `MALER push`
4. Expiration পছন্দ করুন (যেমন 90 days বা No expiration)।
5. Scope এ **repo** টিক দিন (সব sub-option টিকলে ভালো)।
6. **Generate token** চাপুন।
7. **টোকেন কপি করে নিরাপদ জায়গায় রাখুন** – একবার দেখানোর পর আবার দেখাবে না।

### ধাপ ২: পুশ করার সময় টোকেন ব্যবহার করুন

PowerShell বা CMD এ:

```bash
cd C:\Users\Adnan\Documents\MAILER
git push -u origin main
```

যদি **Username/Password** জিজ্ঞেস করে:
- **Username:** আপনার GitHub username (`adnanahamed66772ndpc`)
- **Password:** এখানে **পাসওয়ার্ড নয়** – ওপরের **টোকেন** পেস্ট করুন।

একবার সফল পুশ হলে Windows টোকেন সেভ করে রাখতে পারে, পরেরবার আর জিজ্ঞেস নাও করতে পারে।

---

## getaddrinfo() thread failed to start – কী করবেন

এই এরর প্রায়ই **Cursor/VS Code এর ভেতরের টার্মিনাল** থেকে আসে। নিচেরটা চেষ্টা করুন:

### ১. বাইরের টার্মিনাল ব্যবহার করুন (সবচেয়ে কার্যকর)

- **Cursor বন্ধ করুন না**, কিন্তু **Windows এর নিজস্ব টার্মিনাল** খুলুন:
  - **Windows Key** চাপুন, লিখুন **PowerShell** বা **cmd**, তারপর Enter।
  - অথবা **Start menu** → **Windows PowerShell** বা **Command Prompt**।
- সেখানে চালান:
  ```bash
  cd C:\Users\Adnan\Documents\MAILER
  git push -u origin main
  ```
- Username/Password জিজ্ঞেস করলে GitHub **টোকেন** পাসওয়ার্ড হিসেবে দিন।

### ২. SSH দিয়ে পুশ (HTTPS এর বদলে)

যদি উপরেরটাতেও একই এরর আসে, **SSH** ব্যবহার করুন:

1. GitHub এ SSH key অ্যাড করুন: [https://github.com/settings/keys](https://github.com/settings/keys)  
   (নতুন key জেনারেট করতে: `ssh-keygen -t ed25519 -C "your_email"` তারপর `.pub` ফাইলটার কনটেন্ট কপি করে GitHub এ পেস্ট করুন।)
2. তারপর একই **বাইরের PowerShell/CMD** এ:
   ```bash
   cd C:\Users\Adnan\Documents\MAILER
   git remote set-url origin git@github.com:adnanahamed66772ndpc/MALER.git
   git push -u origin main
   ```

### ৩. ডিএনএস ক্লিয়ার

**বাইরের** PowerShell এ (Admin না হলেও হয়):

```powershell
ipconfig /flushdns
cd C:\Users\Adnan\Documents\MAILER
git push -u origin main
```

### ৪. VPN/অ্যান্টিভাইরাস

VPN বন্ধ করে আবার পুশ চেষ্টা করুন। অ্যান্টিভাইরাস/ফায়ারওয়ালে Git বা GitHub ব্লক করছে কিনা দেখুন।

---

## অন্যান্য সমস্যা

যদি অন্য নেটওয়ার্ক এরর আসে, নিচের ধাপগুলো চেষ্টা করুন।

## ১. স্ক্রিপ্ট দিয়ে পুশ (সবচেয়ে সহজ)

রিপো ফোল্ডারে **push-to-github.ps1** ফাইলটি ডাবল-ক্লিক করুন অথবা PowerShell এ লিখুন:

```powershell
cd C:\Users\Adnan\Documents\MAILER
.\push-to-github.ps1
```

## ২. টার্মিনালে সরাসরি

**PowerShell** বা **Command Prompt** খুলে (Cursor/VS Code এর বাইরে, সাধারণ টার্মিনাল):

```bash
cd C:\Users\Adnan\Documents\MAILER
git push -u origin main
```

## ৩. SSH ব্যবহার করুন (HTTPS এরর থাকলে)

GitHub এ SSH key অ্যাড করুন ([GitHub → Settings → SSH and GPG keys](https://github.com/settings/keys)), তারপর:

```bash
cd C:\Users\Adnan\Documents\MAILER
git remote set-url origin git@github.com:adnanahamed66772ndpc/MALER.git
git push -u origin main
```

## ৪. ডিএনএস ও Git সেটিং

```bash
ipconfig /flushdns
git config --global http.postBuffer 524288000
git push -u origin main
```

## ৫. VPN/ফায়ারওয়াল

VPN বন্ধ করে বা মোবাইল ডেটা/অন্য নেটওয়ার্ক দিয়ে আবার পুশ চেষ্টা করুন।

---

রিপো লিংক: [https://github.com/adnanahamed66772ndpc/MALER](https://github.com/adnanahamed66772ndpc/MALER)
