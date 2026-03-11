# Cài đặt danh tính lỡ như máy chưa có
git config user.name "Gia Huy"
git config user.email "giahuy@local"

# Khởi tạo và Commit toàn bộ Code hiện tại
git init
git add .
git commit -m "Khởi tạo dự án Library Management System Architecture"

# Tạo nhánh main và kết nối với Github
git branch -M main
git remote remove origin 2>$null
git remote add origin https://github.com/GiaHuy361/LibraryManagementSystem.git

# Đẩy nhánh main lên Github
Write-Host "Đang đẩy nhánh main lên GitHub..."
git push -u origin main

# Tạo nhánh develop từ main và đẩy lên
git checkout -b develop
Write-Host "Đang đẩy nhánh develop lên GitHub..."
git push -u origin develop

# Tạo các nhánh phụ cho thành viên từ nhánh develop
git branch phuc
git branch lam
git branch luan
git branch nam

# Đẩy các nhánh phụ này lên Github luôn để mọi người có sẵn
Write-Host "Đang đẩy nhánh phuc, lam, luan, nam lên GitHub..."
git push origin phuc lam luan nam

# Cuối cùng, tạo nhánh huy (nhánh của bạn) và đẩy lên. 
# Repo hiện tại ở local của bạn sẽ nằm cố định ở nhánh huy này để bạn thao tác.
git checkout -b huy
Write-Host "Đang đẩy nhánh huy lên GitHub..."
git push -u origin huy

Write-Host "HOÀN TẤT SETUP GIT TOÀN BỘ NHÁNH!!!"
