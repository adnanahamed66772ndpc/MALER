# Windows যাতে অ্যাপ ব্লক না করে (SmartScreen / Block না হওয়া)

Windows অনেক সময় অপরিচিত বা **unsigned** (সাইন না করা) অ্যাপ চালাতে গেলে "Windows protected your PC" বা "Unknown publisher" দেখিয়ে ব্লক বা সতর্কতা দেয়। আপনার Mailer অ্যাপ যাতে Windows জেট ব্লক না করে, নিচের পদক্ষেপগুলো নিন।

---

## ১. অ্যাপ ম্যানিফেস্ট (ইতিমধ্যে যোগ করা আছে)

প্রজেক্টে `app.manifest` যোগ করা আছে যাতে:

- অ্যাপ **asInvoker** হিসেবে চলে (অ্যাডমিন দাবি করে না) – এতে Windows অ্যাপটাকে কম “সন্দেহজনক” দেখে।
- Windows 10/11 এর সাথে compatibility ঘোষণা করা আছে।

এটা অ্যাপটাকে সাধারণ ডেস্কটপ অ্যাপ হিসেবে চিনতে Windows কে সাহায্য করে।

---

## ২. Code Signing করলে Windows ব্লক করবে না (সবচেয়ে কার্যকর)

অ্যাপ বা ইনস্টলার **সাইন** করলে Windows SmartScreen সাধারণত ব্লক করে না এবং "Unknown publisher" ওঠে না।

### কী দরকার

- একটি **Code Signing Certificate** (যেকোনো বিশ্বস্ত CA থেকে, যেমন DigiCert, Sectigo, SSL.com)।
- **EV Code Signing** করলে প্রথম দিন থেকেই SmartScreen reputation দ্রুত ভালো হয়; সাধারণ Code Signing করলেও কয়েক সপ্তাহ/মাস ব্যবহারের পর reputation বাড়ে।

### কী সাইন করবেন

1. **পাবলিশ করা EXE**  
   `MailerApp.Desktop.exe` (যে এক্সই ইউজার চালায়)।

2. **ইনস্টলার**  
   Inno Setup দিয়ে বানানো `MailerApp-Setup-X.X.X.exe`।

দুটোই সাইন করলে সবচেয়ে ভালো।

### সাইন করার উপায় (Windows)

1. **SignTool** (Visual Studio বা Windows SDK এর সাথে আসে):

   ```cmd
   signtool sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /f "path\to\your-certificate.pfx" /p "cert-password" "path\to\MailerApp.Desktop.exe"
   ```

   ইনস্টলার সাইন করতে একই কমান্ডে এক্সই পাথটা বদলে দিন।

2. **Inno Setup** দিয়ে বানানোর পর সাইন করতে পারেন:

   ```cmd
   signtool sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /f "YourCert.pfx" /p "password" "installer\output\MailerApp-Setup-1.0.0.exe"
   ```

সাইন করার পর এক্সই/ইনস্টলার ডিস্ট্রিবিউট করলে Windows সাধারণত ব্লক করবে না (এবং জেট “block” করার মতো সতর্কতা কম দেখাবে)।

---

## ৩. সাইন না করলে ইউজার কী করবে (Run anyway)

যদি এখনো সাইন না করেন, অনেক ইউজারের কাছে প্রথম চালানোর সময় এমন সতর্কতা আসতে পারে:

- **"Windows protected your PC"** বা  
- **"This app was blocked because it could harm your device"**

ইউজারদের বলুন:

1. **"More info"** ক্লিক করুন।
2. তারপর **"Run anyway"** ক্লিক করুন।

একবার "Run anyway" দিলে পরবর্তীতে সেই একই এক্সই/ইনস্টলার সাধারণত আর ব্লক দেখায় না (লোকালভাবে)।  
তবুও দীর্ঘমেয়াদে অ্যাপ এবং ইনস্টলার **code sign** করলে Windows জেট ব্লক না করার নিশ্চয়তা সবচেয়ে ভালো।

---

## ৪. সংক্ষেপে

| করণীয় | ফলাফল |
|--------|--------|
| **Code Sign (EXE + Installer)** | Windows সাধারণত অ্যাপ/ইনস্টলার ব্লক করবে না, "Unknown publisher" কম দেখা যাবে। |
| **app.manifest (asInvoker, Win10/11)** | অ্যাপটা স্ট্যান্ডার্ড ডেস্কটপ অ্যাপ হিসেবে চেনা যায়, ইতিমধ্যে প্রজেক্টে আছে। |
| **সাইন নেই** | ইউজার "More info" → "Run anyway" দিয়ে চালাতে পারবে; সাইন করলে এই ধাপ লাগে না। |

চাইলে আপনি শুধু ইনস্টলারটাই আগে সাইন করতে পারেন; যারা সরাসরি এক্সই চালায় তাদের জন্যও এক্সই সাইন করলে Windows আরও কম ব্লক করবে।
