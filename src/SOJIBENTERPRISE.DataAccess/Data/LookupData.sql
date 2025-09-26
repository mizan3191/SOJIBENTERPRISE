
-- SuperAdmin
--Super@dmin

SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [Name], [UserName], [Password], [Email], [UserRole], [IsDisable]) VALUES (1, N'Super Admin', N'SuperAdmin', N'JxNGvi6if82fQBQEcrk+LpX5D5NpBRmBy2od3cp8SOU=', N'admin@example.com', 1, 0)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyInfos] ON 

INSERT [dbo].[CompanyInfos] ([Id], [Name], [Phone], [WhatsApp], [WebSiteLink], [Email], [Email2], [DhakaOfficeAddress], [HeadOfficeAddress], [SpecialNotice], [FacebookPage], [FacebookPage2], [FacebookPage3], [Logo], [Image], [Bkash], [Nagad], [Rocket], [CompanyMessage]) VALUES (1, N'M/S RIMON ENTERPRISE', N'01762901080', N'0123456789', N'https://www.boniyadi.com', N'rimontalukder10@gmail.com', N'support@boniyadi.com', N'Amuakanda,Phulpur,Mymensingh', N'Amuakanda,Phulpur,Mymensingh', N'Welcome to Business', N'https://facebook.com/boniyadi', N'', N'', NULL, NULL, N'017XXXXXXXX', N'017XXXXXXXX', N'017XXXXXXXX', NULL)
SET IDENTITY_INSERT [dbo].[CompanyInfos] OFF
GO


INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'HOS', N'Head Of Sales', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (2, N'ZSM', N'Zonal Sales Manager', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (3, N'ASM', N'Area Sales Manager', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (4, N'TSM', N'Territory Sales Manage', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (5, N'SR', N'Sales Representative', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (6, N'DSR', N'Daily Sales Report', 0);

INSERT [dbo].[CustomerType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (7, N'OWNER', N'Distributor', 0);
GO

INSERT [dbo].[DailyExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'Fuel', N'Fuel costs for transport or generators', 0);

INSERT [dbo].[DailyExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (2, N'Personal Due', N'Personal Due', 0);

INSERT [dbo].[DailyExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (3, N'Bazar Khoroc', N'Bazar Khoroc', 0);

INSERT [dbo].[DailyExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (4, N'Guest Entertainment', N'Expenses for guest hospitality', 0);
GO



INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'Electricity Bill', N'Monthly electricity expenses', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (2, N'Water Bill', N'Monthly water usage charges', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (3, N'Internet Bill', N'Monthly internet charges', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (4, N'Staff Salary', N'Salaries for staff members', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (5, N'Office Supplies', N'Stationery and office materials', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (7, N'Home Rent', N'Monthly house rent', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (8, N'Repairs & Maintenance', N'Expenses for repair and upkeep', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (10, N'Travel', N'Business travel expenses', 0);

INSERT [dbo].[ExpenseType] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (11, N'Miscellaneous', N'Other uncategorized expenses', 0);
GO


INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'Boxes', N'Boxes', 0);

INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (3, N'Can', N'Can', 0);

INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (5, N'Jars', N'Jars', 0);

INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (6, N'Bottles', N'Bottles', 0);

INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (7, N'Tubes', N'Tubes', 0);

INSERT [dbo].[Packaging] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (10, N'Packs', N'Packs', 0);
GO


INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'PBL', N'Pubali Bank Ltd', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (2, N'Islami Bank Ltd', N'Islami Bank Ltd', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (3, N'BKash', N'BKash', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (4, N'Rocket', N'Dutch-Bangla Bank mobile banking service', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (5, N'Sure Cash', N'Mobile banking service used by several banks', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (6, N'mCash', N'Mobile banking service by Islami Bank Bangladesh', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (7, N'UCash', N'United Commercial Bank mobile financial service', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (8, N'First Cash', N'Payment service by First Security Islami Bank', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (9, N'OK Banking', N'Mobile banking by ONE Bank Limited', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (17, N'Account/Card', N'Direct bank account or card payment', 0);
GO


--Below data must be insert 11 to 16
INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (1, N'Cash', N'Cash', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (10, N'DRS Payment', N'DRS Payment', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (11, N'Return Product', N'Return Product', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (12, N'Damage Product', N'Damage Product Adjustment', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (13, N'SR Discount', N'SR Discount', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (14, N'Shop Due', N'Shop Due', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (15, N'Daily Expense', N'Daily Expense', 0);

INSERT [dbo].[PaymentMethod] ([Id], [Name], [Description], [IsDeleted]) 
VALUES (16, N'Claim', N'Company Discound Value Added', 0);
GO



INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (10, N'100g', N'100 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (11, N'250gm', N'250 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (12, N'500gm', N'500 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (13, N'1kg', N'1 kilogram', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (14, N'5kg', N'5 kilograms', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (15, N'200gm', N'200 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (16, N'50gm', N'50 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (17, N'400gm', N'400 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (18, N'225gm', N'225 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (19, N'135gm', N'135 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (20, N'176gm', N'176 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (21, N'20gm', N'20 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (22, N'XXL', N'XXL', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (23, N'XL', N'XL', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (24, N'L', N'Large', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (25, N'M', N'Medium', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (26, N'S', N'Small', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (27, N'98gm', N'98 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (28, N'10gm', N'10 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (29, N'33gm', N'33 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (30, N'35gm', N'35 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (31, N'62gm', N'62 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (32, N'150gm', N'150 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (33, N'300gm', N'300 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (34, N'8gm', N'8 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (35, N'12gm', N'12 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (36, N'25gm', N'25 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (37, N'240gm', N'240 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (38, N'90gm', N'90 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (39, N'68gm', N'68 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (40, N'180gm', N'180 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (41, N'80gm', N'80 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (42, N'290gm', N'290 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (43, N'130gm', N'130 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (44, N'496gm', N'496 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (45, N'744gm', N'744 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (46, N'248gm', N'248 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (47, N'140gm', N'140 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (48, N'73gm', N'73 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (49, N'175gm', N'175 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (50, N'70gm', N'70 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (51, N'40gm', N'40 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (52, N'30gm', N'30 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (53, N'5gm', N'5 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (54, N'Single', N'Single', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (55, N'100ml', N'100 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (56, N'200ml', N'200 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (57, N'75gm', N'75 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (58, N'5.5ml', N'5.5 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (59, N'3ml', N'3 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (60, N'10ml', N'10 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (61, N'18ml', N'18 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (62, N'120pcs', N'120pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (63, N'20pcs', N'20pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (64, N'160pcs', N'160pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (65, N'240pcs', N'240pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (66, N'15pcs', N'15pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (67, N'8pcs', N'8pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (68, N'10pcs', N'10pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (69, N'5pcs', N'5pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (70, N'22gm', N'22 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (71, N'15gm', N'15 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (72, N'26gm', N'26 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (73, N'4gm', N'4 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (74, N'80ml', N'80 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (75, N'500ml', N'500 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (76, N'120gm', N'120 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (77, N'340gm', N'340 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (78, N'90pcs', N'90 pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (79, N'110pcs', N'110 pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (80, N'180pcs', N'180 pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (81, N'200pcs', N'200 pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (82, N'80pcs', N'80 pcs', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (83, N'214gm', N'214 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (84, N'340ml', N'170 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (85, N'170ml', N'170 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (86, N'5.25ml', N'5.25 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (87, N'40ml', N'40 milliliter', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (88, N'18gm', N'18 grams', 0);
INSERT [dbo].[ProductsSize] ([Id], [Name], [Description], [IsDeleted]) VALUES (89, N'16pcs', N'16 pcs', 0);
GO


INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (1, N'Damage', N'Damage', 0);
INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (2, N'Incentive', N'Incentive', 0);
INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (3, N'Claim', N'Claim', 0);
INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (5, N'Stock Correction (Overstated Inventory)', N'Adjustment due to incorrect inventory count', 0);
INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (6, N'Warehouse Transfer Out', N'Stock moved to another warehouse', 0);
INSERT [dbo].[ReasonofAdjustment] ([Id], [Name], [Description], [IsDeleted]) VALUES (8, N'Donation/Gift', N'Product donated or gifted', 0);
GO


INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (1, N'Partola', N'', 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (2, N'Balia', N'', 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (3, N'Ramvodropur', N'', 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (4, N'Vaitkandi', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (5, N'Kakni', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (6, N'Sorchapur', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (7, N'Dhara-Nagla', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (8, N'Haluagat', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (9, N'Tarakanda', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (10, N'Phulpur', NULL, 0)
INSERT [dbo].[Road] ([Id], [Name], [Description], [IsDeleted]) VALUES (11, N'Dhubaura', NULL, 0)
GO

INSERT [dbo].[ShippingMethod] ([Id], [Name], [Description], [IsDeleted]) VALUES (1, N'Company Car', N'Company Car', 0)
INSERT [dbo].[ShippingMethod] ([Id], [Name], [Description], [IsDeleted]) VALUES (4, N'Self Pickup', N'Self Pickup', 0)
INSERT [dbo].[ShippingMethod] ([Id], [Name], [Description], [IsDeleted]) VALUES (5, N'Duel Transport', N'Duel Transport', 0)
INSERT [dbo].[ShippingMethod] ([Id], [Name], [Description], [IsDeleted]) VALUES (6, N'Anis Transport', N'Anis Transport', 0)
GO


INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (1, N'pcs', N'piece', 0)
INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (3, N'gm', N'gram', 0)
INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (4, N'l', N'liter', 0)
INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (5, N'kg', N'Kilogram', 0)
INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (6, N'ml', N'milliliter', 0)
INSERT [dbo].[UnitOfMeasurement] ([Id], [Name], [Description], [IsDeleted]) VALUES (7, N'gm', N'miligram', 0)
GO


















--INSERT INTO PriceHistories (
--    BuyingOldPrice,
--    BuyingNewPrice,
--    SellingOldPrice,
--    SellingNewPrice,
--    Date,
--    ProductId
--)
--SELECT 
--    BuyingPrice AS BuyingOldPrice,
--    BuyingPrice AS BuyingNewPrice,
--    SellingPrice AS SellingOldPrice,
--    SellingPrice AS SellingNewPrice,
--    CAST(GETDATE() AS DATE) AS Date,
--    Id AS ProductId
--FROM Products;

--UPDATE Products
--SET StockQty = 0;

-- These tables likely have foreign key constraints, so use DELETE + RESEED
--DELETE FROM TransactionHistories;
--DBCC CHECKIDENT ('TransactionHistories', RESEED, 0);

--DELETE FROM SupplierPaymentHistories;
--DBCC CHECKIDENT ('SupplierPaymentHistories', RESEED, 0);

--DELETE FROM CustomerPaymentHistories;
--DBCC CHECKIDENT ('CustomerPaymentHistories', RESEED, 0);

--DELETE FROM SRPaymentHistories;
--DBCC CHECKIDENT ('SRPaymentHistories', RESEED, 0);


--DELETE FROM SRDiscounts;
--DBCC CHECKIDENT ('SRDiscounts', RESEED, 0);

--DELETE FROM PurchaseDetails;
--DBCC CHECKIDENT ('PurchaseDetails', RESEED, 0);

--DELETE FROM Purchases;
--DBCC CHECKIDENT ('Purchases', RESEED, 0);

--DELETE FROM ProductConsumptions;
--DBCC CHECKIDENT ('ProductConsumptions', RESEED, 0);


--DELETE FROM Expenses;
--DBCC CHECKIDENT ('Expenses', RESEED, 0);


--DELETE FROM DSRShopPaymentHistories;
--DBCC CHECKIDENT ('DSRShopPaymentHistories', RESEED, 0);


--DELETE FROM DSRShopDues;
--DBCC CHECKIDENT ('DSRShopDues', RESEED, 0);

--DELETE FROM DamageProducts;
--DBCC CHECKIDENT ('DamageProducts', RESEED, 0);

--DELETE FROM DamageProductReturns;
--DBCC CHECKIDENT ('DamageProductReturns', RESEED, 0);

--DELETE FROM DamageProductReturnDetails;
--DBCC CHECKIDENT ('DamageProductReturnDetails', RESEED, 0);

--DELETE FROM DamageProductHandovers;
--DBCC CHECKIDENT ('DamageProductHandovers', RESEED, 0);

--DELETE FROM DamageProductHandoverPaymentHistories;
--DBCC CHECKIDENT ('DamageProductHandoverPaymentHistories', RESEED, 0);

--DELETE FROM DamageProductHandoverDetails;
--DBCC CHECKIDENT ('DamageProductHandoverDetails', RESEED, 0);

--DELETE FROM CustomerProductReturns;
--DBCC CHECKIDENT ('CustomerProductReturns', RESEED, 0);

--DELETE FROM CustomerProductReturnDetails;
--DBCC CHECKIDENT ('CustomerProductReturnDetails', RESEED, 0);


--DELETE FROM FreeProductOffers;
--DBCC CHECKIDENT ('FreeProductOffers', RESEED, 0);

--DELETE FROM OrderDetails;
--DBCC CHECKIDENT ('OrderDetails', RESEED, 0);

--DELETE FROM OrderPaymentHistories;
--DBCC CHECKIDENT ('OrderPaymentHistories', RESEED, 0);

--DELETE FROM DailyExpenses;
--DBCC CHECKIDENT ('DailyExpenses', RESEED, 0);


--DELETE FROM Orders;
--DBCC CHECKIDENT ('Orders', RESEED, 0);


