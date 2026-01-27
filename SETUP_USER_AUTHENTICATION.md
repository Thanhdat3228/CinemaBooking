# Hướng Dẫn Cập Nhật Database

## Các Bước Thực Hiện

### 1. Tạo Migration Mới

Mở Terminal/Command Prompt trong thư mục dự án và chạy:

```bash
dotnet ef migrations add AddUserAuthenticationAndUpdate
```

### 2. Cập Nhật Database

```bash
dotnet ef database update
```

### 3. Cài Đặt Package BCrypt (Nếu Chưa Có)

```bash
dotnet add package BCrypt.Net-Next
```

## Các Thay Đổi Trong Database

### User Table

- Thêm: `PasswordHash` (SHA256)
- Thêm: `FullName` (string)
- Thêm: `PhoneNumber` (string)
- Thêm: `CreatedAt` (DateTime)
- Thêm: `UpdatedAt` (DateTime nullable)
- Xóa: `Password` (cũ)

### ShowTime Table

- Thêm: `StartTime` (DateTime)
- Thêm: `RoomNumber` (string)

### Seat Table

- Thêm: `SeatType` (string - Normal/VIP/Couple)
- Thêm: `Price` (decimal)
- Thêm: `IsAvailable` (bool)

### Cinema Table

- Thêm: `Location` (string)
- Thêm: `Phone` (string)

## Tính Năng Hoàn Thành

### 1. Authentication (Đăng Ký/Đăng Nhập)

- ✅ Đăng ký tài khoản mới
- ✅ Mã hóa mật khẩu với BCrypt
- ✅ Đăng nhập với xác thực email/mật khẩu
- ✅ Quản lý Session người dùng
- ✅ Đăng xuất tài khoản

### 2. User Profile (Quản Lý Thông Tin)

- ✅ Xem thông tin cá nhân
- ✅ Chỉnh sửa thông tin
- ✅ Thay đổi mật khẩu
- ✅ Lịch sử tạo/cập nhật tài khoản

### 3. Booking History (Lịch Sử Đặt Vé)

- ✅ Xem danh sách vé đã đặt
- ✅ Xem chi tiết từng vé
- ✅ Hủy vé (nếu suất chiếu chưa bắt đầu)
- ✅ Phân loại vé sắp tới/đã xem

### 4. Dashboard (Trang Tổng Quan)

- ✅ Thống kê: Tổng vé, vé sắp tới, vé đã xem
- ✅ Danh sách vé gần đây
- ✅ Nút hành động nhanh

## Menu Navigation

Navbar đã được cập nhật với:

- Nút "Danh Sách Phim"
- Menu dropdown cho người dùng đã đăng nhập
- Các liên kết đến các trang quản lý

## Session Management

- Timeout: 2 giờ không hoạt động
- Lưu trữ: UserId, UserEmail, UserFullName

## Bảo Mật

- Mật khẩu được mã hóa với BCrypt
- Session bảo vệ với HttpOnly cookie
- Xác thực trước khi truy cập trang người dùng
- CSRF Token cho tất cả form

## Sử Dụng

### Đăng Ký

- URL: `/Auth/Register`
- Form: Email, Họ Tên, Số ĐT (tùy chọn), Mật khẩu

### Đăng Nhập

- URL: `/Auth/Login`
- Form: Email, Mật khẩu, Ghi nhớ

### Quản Lý Thông Tin

- URL: `/User/Index` (Xem)
- URL: `/User/Edit` (Sửa)
- URL: `/Auth/ChangePassword` (Đổi mật khẩu)

### Lịch Sử Đặt Vé

- URL: `/User/BookingHistory` (Danh sách)
- URL: `/User/BookingDetail/{id}` (Chi tiết)

### Dashboard

- URL: `/User/Dashboard` (Trang tổng quan)

## Lưu Ý Quan Trọng

1. **Chạy Migration**: Bạn PHẢI chạy migration để cập nhật schema database
2. **Cài Đặt BCrypt**: Dự án sử dụng BCrypt.Net-Next để mã hóa mật khẩu
3. **Session**: Đảm bảo session middleware được thêm vào Program.cs
4. **Kiểm Tra Connection String**: Đảm bảo connection string trong appsettings.json đúng

## Testing

### Test Đăng Ký

1. Truy cập `/Auth/Register`
2. Nhập thông tin: email, tên, số ĐT, mật khẩu
3. Đăng ký thành công sẽ redirect tới trang đăng nhập

### Test Đăng Nhập

1. Truy cập `/Auth/Login`
2. Nhập email và mật khẩu vừa tạo
3. Thành công sẽ redirect tới Home

### Test Dashboard

1. Sau khi đăng nhập, click "Trang Tổng Quan"
2. Sẽ thấy thống kê vé

### Test Booking History

1. Sau khi đăng nhập, click "Lịch Sử Đặt Vé"
2. Sẽ thấy danh sách vé đã đặt (nếu có)
