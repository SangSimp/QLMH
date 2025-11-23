-- Thay email này bằng email của tài khoản mới
SELECT Id FROM dbo.AspNetUsers WHERE Email = 'abc@gmail.com';
SELECT Id FROM dbo.AspNetRoles WHERE Name = 'Admin';
INSERT INTO dbo.AspNetUserRoles (UserId, RoleId)
VALUES 
(
    'c392565d-fe6d-49db-8266-c69ef5b58b23',  -- << DÁN UserId (từ Lệnh 1) VÀO ĐÂY
    '8dd16a3c-a98e-4aef-a517-c50d40e4f3c4'   -- << DÁN RoleId (từ Lệnh 2) VÀO ĐÂY
);
SELECT * FROM dbo.Reviews
ORDER BY Id DESC; -- Sắp xếp để thấy review mới nhất ở trên cùng