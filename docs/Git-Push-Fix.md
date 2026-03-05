# Git push না হলে কী করবেন

যদি `git push -u origin main` চালালে **getaddrinfo() thread failed to start** বা অন্য নেটওয়ার্ক এরর আসে, নিচের ধাপগুলো চেষ্টা করুন।

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
