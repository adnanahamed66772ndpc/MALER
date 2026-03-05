# Git দিয়ে অ্যাপ/EXE তৈরি (GitHub Actions)

রিপোতে push করলেই GitHub অটো বিল্ড চালিয়ে EXE তৈরি করবে। বিল্ড শেষে **Artifacts** থেকে ZIP ডাউনলোড করে নিলেই হবে।

## কীভাবে EXE পাবেন

1. **কোড push করুন** (master বা main ব্রাঞ্চে):
   ```bash
   git add -A
   git commit -m "Update"
   git push origin master
   ```

2. **GitHub এ যান:**  
   [https://github.com/adnanahamed66772ndpc/MALER](https://github.com/adnanahamed66772ndpc/MALER)  
   → **Actions** ট্যাবে ক্লিক করুন।

3. **সবচেয়ে উপরের workflow run** এ ক্লিক করুন (যেটা সবুজ চেকমার্ক দেখাচ্ছে)।

4. **নিচে স্ক্রল করে "Artifacts"** দেখুন। সেখানে থাকবে:
   - **MailerApp-Desktop-EXE** – ডেস্কটপ অ্যাপ (EXE + সব ফাইল) একটা ZIP।
   - **MailerApp-Worker-EXE** – Worker অ্যাপের ZIP (ইমেল সেন্ড করার জন্য আলাদা চালানোর জন্য)।

5. **MailerApp-Desktop-EXE** ডাউনলোড করুন। ZIP আনজিপ করে ভেতরে **MailerApp.Desktop.exe** চালান। পুরো ফোল্ডার কপি করে যেকোনো Windows PC তে চালাতে পারবেন।

## ম্যানুয়াল বিল্ড চালানো

- **Actions** → বাম পাশে **"Build and publish EXE"** সিলেক্ট করুন → **"Run workflow"** (ডান পাশে) → **Run workflow**।  
  কোনটা পরিবর্তন না করেই রান করলেই আবার EXE বিল্ড হবে এবং Artifacts এ আসবে।

## নোট

- বিল্ড **Windows** রানারে চলে (`.github/workflows/build.yml`)।
- **.NET 10** SDK ব্যবহার করা হয়। যদি GitHub রানারে 10 না থাকে, workflow ফেইল করলে `build.yml` এ `dotnet-version` পরিবর্তন করে আপনার প্রজেক্টের টার্গেট অনুযায়ী (যেমন `8.0.x`) দিতে পারেন।
- EXE গুলো **self-contained** (যেকোনো PC তে .NET ইন্সটল না থাকলেও চলে)।
