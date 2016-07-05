/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50626
Source Host           : localhost:3306
Source Database       : cyberpark

Target Server Type    : MYSQL
Target Server Version : 50626
File Encoding         : 65001

Date: 2016-04-18 21:14:34
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `RegisterBranchId` varchar(128) NOT NULL,
  `ChargeBranchId` varchar(128) NOT NULL,
  `CustomerId` int(11) NOT NULL,
  `Type` varchar(10) NOT NULL,
  `InvoicePeriodType` varchar(20) NOT NULL,
  `NextInvoiceIssueDate` datetime DEFAULT NULL,
  `Address` varchar(250) NOT NULL,
  `FirstName` varchar(50) DEFAULT NULL,
  `LastName` varchar(50) DEFAULT NULL,
  `Title` varchar(20) DEFAULT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `OrganizationName` varchar(100) DEFAULT NULL,
  `IdentityType` varchar(20) DEFAULT NULL,
  `IdentityNumber` varchar(50) DEFAULT NULL,
  `Balance` double(10,2) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `CustomerId` (`CustomerId`),
  KEY `RegisterBranch` (`RegisterBranchId`),
  KEY `ChargeBranch` (`ChargeBranchId`),
  CONSTRAINT `accounts_ibfk_1` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`Id`),
  CONSTRAINT `accounts_ibfk_2` FOREIGN KEY (`RegisterBranchId`) REFERENCES `branches` (`Id`),
  CONSTRAINT `accounts_ibfk_3` FOREIGN KEY (`ChargeBranchId`) REFERENCES `branches` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=700451 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for account_balance_records
-- ----------------------------
DROP TABLE IF EXISTS `account_balance_records`;
CREATE TABLE `account_balance_records` (
  `Id` varchar(128) NOT NULL,
  `AccountId` int(11) NOT NULL,
  `PreviousBalance` double NOT NULL,
  `TransactionId` varchar(128) DEFAULT NULL,
  `AdjustmentId` varchar(128) DEFAULT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `Operate` varchar(20) NOT NULL,
  `Amount` double NOT NULL,
  `CurrentBalance` double NOT NULL,
  `OperateDate` date NOT NULL,
  `Memo` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `account_balance_records_ibfk_1` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for addon_charge_records
-- ----------------------------
DROP TABLE IF EXISTS `addon_charge_records`;
CREATE TABLE `addon_charge_records` (
  `Id` varchar(128) CHARACTER SET utf8 NOT NULL,
  `FileId` varchar(128) CHARACTER SET utf8 NOT NULL,
  `ChargeDate` datetime NOT NULL,
  `OriNumber` varchar(50) CHARACTER SET utf8 NOT NULL,
  `DateFrom` datetime NOT NULL,
  `DateTo` datetime DEFAULT NULL,
  `Cost` double(10,2) NOT NULL,
  `Charge` double(10,2) NOT NULL,
  `ServiceId` varchar(128) CHARACTER SET utf8 DEFAULT NULL,
  `PhoneType` varchar(20) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `Ignored` bit(1) NOT NULL,
  `IgnoredBy` int(11) DEFAULT NULL,
  `Description` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `DisplayDescription` varchar(100) DEFAULT NULL,
  `Warning` varchar(255) DEFAULT NULL,
  `Memo` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FileId` (`FileId`),
  KEY `ServiceId` (`ServiceId`),
  KEY `addon_service_records_ibfk_5` (`InvoiceId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `addon_charge_records_ibfk_1` FOREIGN KEY (`FileId`) REFERENCES `external_bill_files` (`Id`),
  CONSTRAINT `addon_charge_records_ibfk_4` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`),
  CONSTRAINT `addon_charge_records_ibfk_5` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`),
  CONSTRAINT `addon_charge_records_ibfk_6` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for adjustments
-- ----------------------------
DROP TABLE IF EXISTS `adjustments`;
CREATE TABLE `adjustments` (
  `Id` varchar(128) CHARACTER SET utf8 NOT NULL,
  `AccountId` int(11) NOT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `Amount` double(10,2) NOT NULL,
  `Memo` text CHARACTER SET utf8,
  `OperatedBy` int(11) NOT NULL,
  `OperatedDate` datetime NOT NULL,
  `IsDeleted` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `InvoiceId` (`InvoiceId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `adjustments_ibfk_7` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`),
  CONSTRAINT `adjustments_ibfk_8` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for branches
-- ----------------------------
DROP TABLE IF EXISTS `branches`;
CREATE TABLE `branches` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_charge_records
-- ----------------------------
DROP TABLE IF EXISTS `calling_charge_records`;
CREATE TABLE `calling_charge_records` (
  `Id` varchar(128) NOT NULL,
  `ServiceId` varchar(128) NOT NULL,
  `DateFrom` datetime NOT NULL,
  `DateTo` datetime NOT NULL,
  `Cost` double(11,2) NOT NULL,
  `Charge` double(11,2) NOT NULL,
  `AccountId` int(11) NOT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `OperatedDate` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ServiceId` (`ServiceId`),
  KEY `InvoiceId` (`InvoiceId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `calling_charge_records_ibfk_1` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`),
  CONSTRAINT `calling_charge_records_ibfk_2` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`),
  CONSTRAINT `calling_charge_records_ibfk_3` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_international_regions
-- ----------------------------
DROP TABLE IF EXISTS `calling_international_regions`;
CREATE TABLE `calling_international_regions` (
  `Id` varchar(20) NOT NULL,
  `Description` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_international_region_details
-- ----------------------------
DROP TABLE IF EXISTS `calling_international_region_details`;
CREATE TABLE `calling_international_region_details` (
  `Id` varchar(128) NOT NULL,
  `RegionId` varchar(20) NOT NULL,
  `CountryName` varchar(100) NOT NULL,
  `IncludeMobile` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `RegionCode` (`RegionId`),
  CONSTRAINT `calling_international_region_details_ibfk_1` FOREIGN KEY (`RegionId`) REFERENCES `calling_international_regions` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_offers
-- ----------------------------
DROP TABLE IF EXISTS `calling_offers`;
CREATE TABLE `calling_offers` (
  `Id` varchar(128) NOT NULL,
  `Type` varchar(20) NOT NULL COMMENT 'PSTN, VoIP or Broadband',
  `Name` varchar(100) NOT NULL,
  `Minutes` int(11) NOT NULL COMMENT 'calling minutes for PSTN, VoIP; -1 = unlimited',
  `Local` bit(1) NOT NULL COMMENT 'flag for whether local call applicable',
  `National` bit(1) NOT NULL COMMENT 'flag for whether national call applicable',
  `Mobile` bit(1) NOT NULL COMMENT 'flag for whether mobile call applicable',
  `CallingRegionId` varchar(20) DEFAULT NULL COMMENT 'applicable calling region',
  `IsActive` bit(1) NOT NULL,
  `OperatorId` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_rates_pstn
-- ----------------------------
DROP TABLE IF EXISTS `calling_rates_pstn`;
CREATE TABLE `calling_rates_pstn` (
  `Prefix` varchar(20) NOT NULL COMMENT '地区前缀',
  `Type` varchar(20) NOT NULL COMMENT '费率类型',
  `AreaName` varchar(50) NOT NULL COMMENT '地区名称',
  `RatePerMinute` double(10,3) NOT NULL COMMENT '计费费率',
  PRIMARY KEY (`Prefix`,`Type`),
  KEY `area_prefix` (`Prefix`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_rates_voip
-- ----------------------------
DROP TABLE IF EXISTS `calling_rates_voip`;
CREATE TABLE `calling_rates_voip` (
  `Prefix` varchar(10) NOT NULL COMMENT '地区前缀',
  `Type` varchar(50) NOT NULL COMMENT '费率类型',
  `AreaName` varchar(50) NOT NULL COMMENT '地区名称',
  `RatePerMinute` double(10,3) NOT NULL COMMENT '分钟费用'
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for calling_records
-- ----------------------------
DROP TABLE IF EXISTS `calling_records`;
CREATE TABLE `calling_records` (
  `Id` varchar(128) CHARACTER SET utf8 NOT NULL,
  `FileId` varchar(128) CHARACTER SET utf8 NOT NULL,
  `OriNumber` varchar(50) CHARACTER SET utf8 NOT NULL,
  `DesNumber` varchar(50) CHARACTER SET utf8 DEFAULT NULL,
  `AreaPrefix` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `AreaName` varchar(50) DEFAULT NULL,
  `Type` varchar(20) CHARACTER SET utf8 NOT NULL,
  `IsMobile` bit(1) NOT NULL,
  `CallStart` datetime NOT NULL,
  `Duration` int(11) NOT NULL,
  `Cost` double(10,2) NOT NULL,
  `RatePerMinute` double(11,4) NOT NULL,
  `ChargeMinute` int(11) NOT NULL,
  `Charge` double(10,2) NOT NULL,
  `ActualChargeMiute` int(11) DEFAULT NULL,
  `ActualCharge` double(10,2) DEFAULT NULL,
  `OfferId` varchar(128) CHARACTER SET utf8 DEFAULT NULL,
  `ServiceId` varchar(128) CHARACTER SET utf8 DEFAULT NULL,
  `PhoneType` varchar(20) CHARACTER SET utf8 DEFAULT NULL,
  `ChargeRecordId` varchar(128) CHARACTER SET utf8 DEFAULT NULL,
  `Ignored` bit(1) NOT NULL,
  `IgnoredBy` int(11) DEFAULT NULL,
  `Description` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Warning` varchar(255) DEFAULT NULL,
  `Memo` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FileId` (`FileId`),
  KEY `ServiceID` (`ServiceId`),
  KEY `OfferId` (`OfferId`),
  KEY `ChargeRecordId` (`ChargeRecordId`),
  CONSTRAINT `calling_records_ibfk_1` FOREIGN KEY (`FileId`) REFERENCES `external_bill_files` (`Id`),
  CONSTRAINT `calling_records_ibfk_10` FOREIGN KEY (`ChargeRecordId`) REFERENCES `calling_charge_records` (`Id`),
  CONSTRAINT `calling_records_ibfk_4` FOREIGN KEY (`ServiceId`) REFERENCES `services` (`Id`),
  CONSTRAINT `calling_records_ibfk_7` FOREIGN KEY (`OfferId`) REFERENCES `service_usage_offers` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS `customers`;
CREATE TABLE `customers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) DEFAULT NULL,
  `Title` varchar(20) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `IdentityType` varchar(20) DEFAULT NULL,
  `IdentityNumber` varchar(50) DEFAULT NULL,
  `SignUpDate` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=600436 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for equipments
-- ----------------------------
DROP TABLE IF EXISTS `equipments`;
CREATE TABLE `equipments` (
  `Id` varchar(128) NOT NULL COMMENT 'equipment serial number',
  `ModelId` varchar(128) DEFAULT NULL,
  `StoredDate` date NOT NULL,
  `ProductId` varchar(128) DEFAULT NULL,
  `DispatchedDate` datetime DEFAULT NULL,
  `Status` varchar(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ProductId` (`ProductId`),
  KEY `equipments_ibfk_2` (`ModelId`),
  CONSTRAINT `equipments_ibfk_2` FOREIGN KEY (`ModelId`) REFERENCES `equipment_models` (`Id`),
  CONSTRAINT `equipments_ibfk_3` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for equipment_models
-- ----------------------------
DROP TABLE IF EXISTS `equipment_models`;
CREATE TABLE `equipment_models` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Type` varchar(20) NOT NULL,
  `Price` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for external_bill_addon_converters
-- ----------------------------
DROP TABLE IF EXISTS `external_bill_addon_converters`;
CREATE TABLE `external_bill_addon_converters` (
  `Id` varchar(128) NOT NULL,
  `Keywords` varchar(255) NOT NULL,
  `DisplayAs` varchar(255) DEFAULT NULL,
  `Price` double(11,2) DEFAULT NULL,
  `IsDisplay` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for external_bill_files
-- ----------------------------
DROP TABLE IF EXISTS `external_bill_files`;
CREATE TABLE `external_bill_files` (
  `Id` varchar(128) CHARACTER SET utf8 NOT NULL,
  `FileName` varchar(50) CHARACTER SET utf8 NOT NULL,
  `Source` varchar(10) CHARACTER SET utf8 NOT NULL,
  `Year` int(11) NOT NULL,
  `Month` int(11) NOT NULL,
  `CallOriNumberCount` int(11) NOT NULL,
  `CallRecordCount` int(11) NOT NULL,
  `CallTotalCost` double(10,2) DEFAULT NULL,
  `CallDateFrom` datetime DEFAULT NULL,
  `CallDateTo` datetime DEFAULT NULL,
  `AddonOriNumberCount` int(11) DEFAULT NULL,
  `AddonRecordCount` int(11) DEFAULT NULL,
  `AddonTotalCost` double(10,2) DEFAULT NULL,
  `AddonDateFrom` datetime DEFAULT NULL,
  `AddonDateTo` datetime DEFAULT NULL,
  `Size` double NOT NULL,
  `OperatedBy` int(11) NOT NULL,
  `OperatedDate` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for invoices
-- ----------------------------
DROP TABLE IF EXISTS `invoices`;
CREATE TABLE `invoices` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AccountId` int(11) NOT NULL,
  `Year` int(11) NOT NULL,
  `Month` int(11) NOT NULL,
  `DateFrom` datetime NOT NULL,
  `DateTo` datetime NOT NULL,
  `PreviousBalance` double(10,2) NOT NULL,
  `ChargeAmountExcludeGST` double(10,2) NOT NULL,
  `GST` double(10,2) NOT NULL,
  `ProductAmount` double(10,2) NOT NULL,
  `AddonAmount` double(10,2) NOT NULL,
  `CallingAmount` double(10,2) NOT NULL,
  `AdjustAmount` double(10,2) NOT NULL,
  `TransactionAmount` double(10,2) NOT NULL,
  `Status` varchar(10) NOT NULL,
  `IssuedBy` int(11) NOT NULL,
  `IssuedDate` datetime NOT NULL,
  `DisplayIssuedDate` datetime NOT NULL,
  `AutoDeliver` bit(1) DEFAULT NULL,
  `DeliveredDate` date DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `invoices_ibfk_1` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for plans
-- ----------------------------
DROP TABLE IF EXISTS `plans`;
CREATE TABLE `plans` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(200) NOT NULL COMMENT '产品名称',
  `IsBusiness` bit(1) NOT NULL,
  `BroadbandType` varchar(20) NOT NULL,
  `PstnCount` int(11) NOT NULL COMMENT '表明此plan默认配有几根pstn电话线',
  `VoipCount` int(11) NOT NULL,
  `MonthsOfContract` int(11) NOT NULL,
  `MonthlyPrice` double(11,2) NOT NULL COMMENT '产品销售价格',
  `NewConnectionCharge` double NOT NULL,
  `ModemPrice` double NOT NULL,
  `IsPromotion` bit(1) NOT NULL COMMENT '是否是特价plan',
  `DisplayPriority` int(11) NOT NULL COMMENT 'plan的放在首页的位置排序，数值越小越靠前',
  `Memo` varchar(255) DEFAULT NULL,
  `IsActive` bit(1) NOT NULL COMMENT '产品状态，active，selling, disable',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='tm, 产品表，记录了客户可以看到的产品的参数产品信息';

-- ----------------------------
-- Table structure for products
-- ----------------------------
DROP TABLE IF EXISTS `products`;
CREATE TABLE `products` (
  `Id` varchar(128) NOT NULL,
  `AccountId` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `BasePriceGSTExclusive` double(10,2) NOT NULL,
  `DiscountRate` double(10,4) NOT NULL,
  `NumberOfMonthPerCharge` int(11) NOT NULL COMMENT 'default = 1',
  `ServiceGivenDate` datetime DEFAULT NULL COMMENT 'term start date',
  `ChargedToDate` datetime DEFAULT NULL,
  `IsTermed` bit(1) NOT NULL COMMENT 'whether has term',
  `TermStartDate` datetime DEFAULT NULL,
  `MonthsOfTerm` int(10) DEFAULT NULL COMMENT 'term length in months',
  `IsOneOff` bit(1) NOT NULL COMMENT '1 - monthly; 0 - one-off',
  `OneOffChargeDate` datetime DEFAULT NULL,
  `HasOneOffCharged` bit(1) DEFAULT NULL COMMENT 'indicate whether one-off service package has been charged',
  `AppliedDate` date NOT NULL,
  `PrepayMonths` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `AddressId` (`AccountId`),
  KEY `InvoiceID` (`HasOneOffCharged`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for product_charge_records
-- ----------------------------
DROP TABLE IF EXISTS `product_charge_records`;
CREATE TABLE `product_charge_records` (
  `Id` varchar(128) NOT NULL,
  `ProductId` varchar(128) NOT NULL,
  `AccountId` int(11) NOT NULL,
  `PreviousProductChargedToDate` datetime DEFAULT NULL,
  `CurrentProductChargedToDate` datetime DEFAULT NULL,
  `AmountGSTExclusive` double(11,2) NOT NULL,
  `ChargedDate` datetime NOT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `Memo` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `InvoiceId` (`InvoiceId`) USING BTREE,
  KEY `ServicePackageId` (`ProductId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `product_charge_records_ibfk_1` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`),
  CONSTRAINT `product_charge_records_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `product_charge_records_ibfk_3` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for roles
-- ----------------------------
DROP TABLE IF EXISTS `roles`;
CREATE TABLE `roles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for services
-- ----------------------------
DROP TABLE IF EXISTS `services`;
CREATE TABLE `services` (
  `Id` varchar(128) NOT NULL,
  `ProductId` varchar(128) NOT NULL,
  `Type` varchar(20) NOT NULL COMMENT 'Broadband, Phone, AddOn, Misellanouse',
  `SubType` varchar(20) DEFAULT NULL COMMENT 'ADSL, VDSL, UFB; PSTN, VoIP',
  `IdentityNumber` varchar(20) DEFAULT NULL,
  `BroadbandSVLAN` varchar(10) DEFAULT NULL,
  `BroadbandCVLAN` varchar(10) DEFAULT NULL,
  `BroadbandPPPoeLoginName` varchar(50) DEFAULT NULL,
  `BroadbandPPPoePassword` varchar(50) DEFAULT NULL,
  `VoipPassword` varchar(20) DEFAULT NULL,
  `VoipAssignedDate` datetime DEFAULT NULL,
  `ReadyForServiceDate` datetime DEFAULT NULL,
  `Status` varchar(30) NOT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `OperatorId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `PackageId` (`ProductId`),
  CONSTRAINT `services_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for service_usage_offers
-- ----------------------------
DROP TABLE IF EXISTS `service_usage_offers`;
CREATE TABLE `service_usage_offers` (
  `Id` varchar(128) NOT NULL,
  `ProductId` varchar(128) NOT NULL,
  `ServiceType` varchar(20) NOT NULL COMMENT 'PSTN, VoIP or Broadband',
  `ServiceSubType` varchar(20) DEFAULT NULL,
  `Name` varchar(100) NOT NULL,
  `Minutes` int(11) NOT NULL COMMENT 'calling minutes for PSTN, VoIP; -1 = unlimited',
  `DataFlow` int(11) NOT NULL COMMENT 'GB for brandboad',
  `Local` bit(1) NOT NULL COMMENT 'flag for whether local call applicable',
  `National` bit(1) NOT NULL COMMENT 'flag for whether national call applicable',
  `Mobile` bit(1) NOT NULL COMMENT 'flag for whether mobile call applicable',
  `CallingRegionId` varchar(20) DEFAULT NULL COMMENT 'applicable calling region',
  `IsDeleted` bit(1) NOT NULL,
  `OperatorId` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `PackageId` (`ProductId`) USING BTREE,
  KEY `RegionCode` (`CallingRegionId`) USING BTREE,
  CONSTRAINT `service_usage_offers_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `service_usage_offers_ibfk_3` FOREIGN KEY (`CallingRegionId`) REFERENCES `calling_international_regions` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for staffs
-- ----------------------------
DROP TABLE IF EXISTS `staffs`;
CREATE TABLE `staffs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BranchId` varchar(128) NOT NULL,
  `Name` varchar(100) NOT NULL COMMENT '持有者名称',
  `Email` varchar(100) NOT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10038 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tickets
-- ----------------------------
DROP TABLE IF EXISTS `tickets`;
CREATE TABLE `tickets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Type` varchar(20) NOT NULL,
  `SubType` varchar(20) DEFAULT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `CustomerId` int(11) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `StaffLastViewedDate` datetime DEFAULT NULL,
  `CustomerLastViewedDate` datetime DEFAULT NULL,
  `CreateDate` datetime DEFAULT NULL,
  `ClosedDate` datetime(1) DEFAULT NULL,
  `ClosedBy` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CustomerId` (`CustomerId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `tickets_ibfk_1` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`Id`),
  CONSTRAINT `tickets_ibfk_2` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for ticket_comments
-- ----------------------------
DROP TABLE IF EXISTS `ticket_comments`;
CREATE TABLE `ticket_comments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TicketId` int(11) NOT NULL,
  `Comment` text,
  `CommentDate` datetime NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `CustomerId` int(11) DEFAULT NULL,
  `StaffId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `TicketId` (`TicketId`),
  KEY `CustomerId` (`CustomerId`),
  KEY `StaffId` (`StaffId`),
  CONSTRAINT `ticket_comments_ibfk_1` FOREIGN KEY (`TicketId`) REFERENCES `tickets` (`Id`),
  CONSTRAINT `ticket_comments_ibfk_2` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`Id`),
  CONSTRAINT `ticket_comments_ibfk_3` FOREIGN KEY (`StaffId`) REFERENCES `staffs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for transactions
-- ----------------------------
DROP TABLE IF EXISTS `transactions`;
CREATE TABLE `transactions` (
  `Id` varchar(128) NOT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `InvoiceId` int(11) DEFAULT NULL,
  `Date` datetime NOT NULL,
  `Amount` double(10,2) NOT NULL,
  `Type` varchar(30) NOT NULL,
  `CardHolder` varchar(50) DEFAULT NULL,
  `CardNumber` varchar(20) DEFAULT NULL,
  `DpsTxnRef` varchar(20) DEFAULT NULL,
  `DpsResponse` varchar(20) DEFAULT NULL,
  `TxnMac` varchar(20) DEFAULT NULL,
  `OperatedBy` int(11) NOT NULL,
  `OperatedDate` datetime NOT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `Memo` text,
  PRIMARY KEY (`Id`),
  KEY `InvoiceId` (`InvoiceId`),
  KEY `AccountId` (`AccountId`),
  CONSTRAINT `transactions_ibfk_4` FOREIGN KEY (`InvoiceId`) REFERENCES `invoices` (`Id`),
  CONSTRAINT `transactions_ibfk_5` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for userclaims
-- ----------------------------
DROP TABLE IF EXISTS `userclaims`;
CREATE TABLE `userclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for userlogins
-- ----------------------------
DROP TABLE IF EXISTS `userlogins`;
CREATE TABLE `userlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` int(11) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for userroles
-- ----------------------------
DROP TABLE IF EXISTS `userroles`;
CREATE TABLE `userroles` (
  `UserId` int(11) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `IdentityRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Email` varchar(100) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=600437 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for void_invoices
-- ----------------------------
DROP TABLE IF EXISTS `void_invoices`;
CREATE TABLE `void_invoices` (
  `id` varchar(128) NOT NULL,
  `InvoiceId` int(11) NOT NULL,
  `VoidBy` int(11) NOT NULL,
  `VoidDate` datetime NOT NULL,
  `JsonData` longtext NOT NULL,
  PRIMARY KEY (`id`),
  KEY `InvoiceId` (`InvoiceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for warnings
-- ----------------------------
DROP TABLE IF EXISTS `warnings`;
CREATE TABLE `warnings` (
  `Id` varchar(128) NOT NULL,
  `Module` varchar(50) NOT NULL,
  `Operate` varchar(50) NOT NULL,
  `Message` varchar(255) NOT NULL,
  `CustomerId` int(11) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `CreateDate` datetime NOT NULL,
  `ClearBy` int(11) DEFAULT NULL,
  `ClearDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
