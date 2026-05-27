# UrlShortener

Dự án rút gọn liên kết (URL Shortener) viết bằng C# .NET Core, sử dụng Entity Framework Core và cơ sở dữ liệu PostgreSQL.

## Yêu cầu hệ thống
* [.NET SDK 8.0](https://dotnet.microsoft.com/download) trở lên.
* [PostgreSQL](https://www.postgresql.org/download/) đang chạy tại máy cục bộ (hoặc Docker).

---

## Hướng dẫn cài đặt và chạy dự án

### Bước 1: Clone dự án và khôi phục thư viện
Sau khi clone dự án về máy, di chuyển vào thư mục dự án và khôi phục các gói thư viện NuGet:
```bash
dotnet restore
```

### Bước 2: Cấu hình Cơ sở dữ liệu
Mở file [appsettings.json](UrlShortener/appsettings.json) và cấu hình lại chuỗi kết nối PostgreSQL (ConnectionStrings) phù hợp với thông tin máy của bạn nếu cần:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=url_shortener;Username=postgres;Password=YOUR_PASSWORD"
}
```

### Bước 3: Cập nhật Migration cơ sở dữ liệu
Để tạo các bảng trong cơ sở dữ liệu PostgreSQL, cài đặt công cụ EF Core CLI (nếu chưa cài):
```bash
dotnet tool install --global dotnet-ef
```
Sau đó, chạy migration để tạo cấu trúc cơ sở dữ liệu:
```bash
cd UrlShortener
dotnet ef database update
```

### Bước 4: Chạy dự án
Chạy ứng dụng từ thư mục `UrlShortener`:
```bash
dotnet run
```
Hoặc mở file solution `UrlShortener.sln` bằng Visual Studio / Rider / VS Code và nhấn nút **Run/Debug**.

---

## Cấu trúc thư mục chính
* **[UrlShortener/](UrlShortener/)**: Mã nguồn chính của ứng dụng Web API.
  * **[Data/](UrlShortener/Data/)**: Lớp kết nối database (`AppDbContext`).
  * **[Models/](UrlShortener/Models/)**: Định nghĩa các thực thể (`Url`).
  * **[Services/](UrlShortener/Services/)**: Logic nghiệp vụ rút gọn link và giải mã link.
  * **[Helpers/](UrlShortener/Helpers/)**: Các lớp hỗ trợ (ví dụ mã hóa Base62).
* **[UrlShortener.Tests/](UrlShortener.Tests/)**: Dự án chứa các unit test.
