-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema cms
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema cms
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `cms` DEFAULT CHARACTER SET utf8mb4 ;
USE `cms` ;

-- -----------------------------------------------------
-- Table `cms`.`__efmigrationshistory`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`__efmigrationshistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  PRIMARY KEY (`MigrationId`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_languages`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_languages` (
  `Id` CHAR(36) NOT NULL,
  `Title` VARCHAR(64) NOT NULL,
  `Culture` VARCHAR(6) NULL DEFAULT NULL,
  `IsDefault` TINYINT(1) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_sites`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_sites` (
  `Id` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `Description` VARCHAR(256) NULL DEFAULT NULL,
  `Hostnames` VARCHAR(256) NULL DEFAULT NULL,
  `InternalId` VARCHAR(64) NOT NULL,
  `IsDefault` TINYINT(1) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `Title` VARCHAR(128) NULL DEFAULT NULL,
  `SiteTypeId` VARCHAR(64) NULL DEFAULT NULL,
  `ContentLastModified` DATETIME(6) NULL DEFAULT NULL,
  `Culture` VARCHAR(6) NULL DEFAULT NULL,
  `LogoId` CHAR(36) NULL DEFAULT NULL,
  `LanguageId` CHAR(36) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Sites_InternalId` (`InternalId` ASC) VISIBLE,
  INDEX `IX_Piranha_Sites_LanguageId` (`LanguageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Sites_Piranha_Languages_LanguageId`
    FOREIGN KEY (`LanguageId`)
    REFERENCES `cms`.`piranha_languages` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_aliases`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_aliases` (
  `Id` CHAR(36) NOT NULL,
  `AliasUrl` VARCHAR(256) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `RedirectUrl` VARCHAR(256) NOT NULL,
  `SiteId` CHAR(36) NOT NULL,
  `Type` INT(11) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Aliases_SiteId_AliasUrl` (`SiteId` ASC, `AliasUrl` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Aliases_Piranha_Sites_SiteId`
    FOREIGN KEY (`SiteId`)
    REFERENCES `cms`.`piranha_sites` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_blocks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_blocks` (
  `Id` CHAR(36) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `IsReusable` TINYINT(1) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `Title` VARCHAR(128) NULL DEFAULT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_blockfields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_blockfields` (
  `Id` CHAR(36) NOT NULL,
  `BlockId` CHAR(36) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_BlockFields_BlockId_FieldId_SortOrder` (`BlockId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_BlockFields_Piranha_Blocks_BlockId`
    FOREIGN KEY (`BlockId`)
    REFERENCES `cms`.`piranha_blocks` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pagetypes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pagetypes` (
  `Id` VARCHAR(64) NOT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `CLRType` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pages`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pages` (
  `Id` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `IsHidden` TINYINT(1) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `MetaDescription` VARCHAR(256) NULL DEFAULT NULL,
  `MetaKeywords` VARCHAR(128) NULL DEFAULT NULL,
  `NavigationTitle` VARCHAR(128) NULL DEFAULT NULL,
  `PageTypeId` VARCHAR(64) NOT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  `Published` DATETIME(6) NULL DEFAULT NULL,
  `RedirectType` INT(11) NOT NULL,
  `RedirectUrl` VARCHAR(256) NULL DEFAULT NULL,
  `Route` VARCHAR(256) NULL DEFAULT NULL,
  `SiteId` CHAR(36) NOT NULL,
  `Slug` VARCHAR(128) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `Title` VARCHAR(128) NOT NULL,
  `ContentType` VARCHAR(255) NOT NULL DEFAULT 'Page',
  `OriginalPageId` CHAR(36) NULL DEFAULT NULL,
  `CloseCommentsAfterDays` INT(11) NOT NULL DEFAULT '0',
  `EnableComments` TINYINT(1) NOT NULL DEFAULT '0',
  `Excerpt` LONGTEXT NULL DEFAULT NULL,
  `PrimaryImageId` CHAR(36) NULL DEFAULT NULL,
  `MetaTitle` VARCHAR(128) NULL DEFAULT NULL,
  `OgDescription` VARCHAR(256) NULL DEFAULT NULL,
  `OgImageId` CHAR(36) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `OgTitle` VARCHAR(128) NULL DEFAULT NULL,
  `MetaFollow` TINYINT(1) NULL DEFAULT '1',
  `MetaIndex` TINYINT(1) NULL DEFAULT '1',
  `MetaPriority` DOUBLE NOT NULL DEFAULT '0.5',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Pages_SiteId_Slug` (`SiteId` ASC, `Slug` ASC) VISIBLE,
  INDEX `IX_Piranha_Pages_PageTypeId` (`PageTypeId` ASC) VISIBLE,
  INDEX `IX_Piranha_Pages_ParentId` (`ParentId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Pages_Piranha_PageTypes_PageTypeId`
    FOREIGN KEY (`PageTypeId`)
    REFERENCES `cms`.`piranha_pagetypes` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_Pages_Piranha_Pages_ParentId`
    FOREIGN KEY (`ParentId`)
    REFERENCES `cms`.`piranha_pages` (`Id`),
  CONSTRAINT `FK_Piranha_Pages_Piranha_Sites_SiteId`
    FOREIGN KEY (`SiteId`)
    REFERENCES `cms`.`piranha_sites` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_categories`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_categories` (
  `Id` CHAR(36) NOT NULL,
  `BlogId` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `Slug` VARCHAR(64) NOT NULL,
  `Title` VARCHAR(64) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Categories_BlogId_Slug` (`BlogId` ASC, `Slug` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Categories_Piranha_Pages_BlogId`
    FOREIGN KEY (`BlogId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contenttypes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contenttypes` (
  `Id` VARCHAR(64) NOT NULL,
  `Group` VARCHAR(64) NOT NULL,
  `CLRType` LONGTEXT NULL DEFAULT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_taxonomies`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_taxonomies` (
  `Id` CHAR(36) NOT NULL,
  `GroupId` VARCHAR(64) NOT NULL,
  `Type` INT(11) NOT NULL,
  `Title` VARCHAR(64) NOT NULL,
  `Slug` VARCHAR(64) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Taxonomies_GroupId_Type_Slug` (`GroupId` ASC, `Type` ASC, `Slug` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_content`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_content` (
  `Id` CHAR(36) NOT NULL,
  `CategoryId` CHAR(36) NULL DEFAULT NULL,
  `TypeId` VARCHAR(64) NOT NULL,
  `PrimaryImageId` CHAR(36) NULL DEFAULT NULL,
  `Excerpt` LONGTEXT NULL DEFAULT NULL,
  `Title` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_Content_CategoryId` (`CategoryId` ASC) VISIBLE,
  INDEX `IX_Piranha_Content_TypeId` (`TypeId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Content_Piranha_ContentTypes_TypeId`
    FOREIGN KEY (`TypeId`)
    REFERENCES `cms`.`piranha_contenttypes` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_Content_Piranha_Taxonomies_CategoryId`
    FOREIGN KEY (`CategoryId`)
    REFERENCES `cms`.`piranha_taxonomies` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentblocks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentblocks` (
  `Id` CHAR(36) NOT NULL,
  `ContentId` CHAR(36) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_ContentBlocks_ContentId` (`ContentId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentBlocks_Piranha_Content_ContentId`
    FOREIGN KEY (`ContentId`)
    REFERENCES `cms`.`piranha_content` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentblockfields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentblockfields` (
  `Id` CHAR(36) NOT NULL,
  `BlockId` CHAR(36) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_ContentBlockFields_BlockId_FieldId_SortOrder` (`BlockId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentBlockFields_Piranha_ContentBlocks_BlockId`
    FOREIGN KEY (`BlockId`)
    REFERENCES `cms`.`piranha_contentblocks` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentblockfieldtranslations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentblockfieldtranslations` (
  `FieldId` CHAR(36) NOT NULL,
  `LanguageId` CHAR(36) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`FieldId`, `LanguageId`),
  INDEX `IX_Piranha_ContentBlockFieldTranslations_LanguageId` (`LanguageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentBlockFieldTranslations_Piranha_ContentBlockFi~`
    FOREIGN KEY (`FieldId`)
    REFERENCES `cms`.`piranha_contentblockfields` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_ContentBlockFieldTranslations_Piranha_Languages_Lang~`
    FOREIGN KEY (`LanguageId`)
    REFERENCES `cms`.`piranha_languages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentfields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentfields` (
  `Id` CHAR(36) NOT NULL,
  `ContentId` CHAR(36) NOT NULL,
  `RegionId` VARCHAR(64) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_ContentFields_ContentId_RegionId_FieldId_SortOrder` (`ContentId` ASC, `RegionId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentFields_Piranha_Content_ContentId`
    FOREIGN KEY (`ContentId`)
    REFERENCES `cms`.`piranha_content` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentfieldtranslations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentfieldtranslations` (
  `FieldId` CHAR(36) NOT NULL,
  `LanguageId` CHAR(36) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`FieldId`, `LanguageId`),
  INDEX `IX_Piranha_ContentFieldTranslations_LanguageId` (`LanguageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentFieldTranslations_Piranha_ContentFields_Field~`
    FOREIGN KEY (`FieldId`)
    REFERENCES `cms`.`piranha_contentfields` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_ContentFieldTranslations_Piranha_Languages_LanguageId`
    FOREIGN KEY (`LanguageId`)
    REFERENCES `cms`.`piranha_languages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contentgroups`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contentgroups` (
  `Id` VARCHAR(64) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `CLRType` VARCHAR(255) NOT NULL,
  `Title` VARCHAR(128) NOT NULL,
  `Icon` VARCHAR(64) NULL DEFAULT NULL,
  `IsHidden` TINYINT(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contenttaxonomies`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contenttaxonomies` (
  `ContentId` CHAR(36) NOT NULL,
  `TaxonomyId` CHAR(36) NOT NULL,
  PRIMARY KEY (`ContentId`, `TaxonomyId`),
  INDEX `IX_Piranha_ContentTaxonomies_TaxonomyId` (`TaxonomyId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentTaxonomies_Piranha_Content_ContentId`
    FOREIGN KEY (`ContentId`)
    REFERENCES `cms`.`piranha_content` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_ContentTaxonomies_Piranha_Taxonomies_TaxonomyId`
    FOREIGN KEY (`TaxonomyId`)
    REFERENCES `cms`.`piranha_taxonomies` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_contenttranslations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_contenttranslations` (
  `ContentId` CHAR(36) NOT NULL,
  `LanguageId` CHAR(36) NOT NULL,
  `Title` VARCHAR(128) NOT NULL,
  `Excerpt` LONGTEXT NULL DEFAULT NULL,
  `LastModified` DATETIME(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  PRIMARY KEY (`ContentId`, `LanguageId`),
  INDEX `IX_Piranha_ContentTranslations_LanguageId` (`LanguageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_ContentTranslations_Piranha_Content_ContentId`
    FOREIGN KEY (`ContentId`)
    REFERENCES `cms`.`piranha_content` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_ContentTranslations_Piranha_Languages_LanguageId`
    FOREIGN KEY (`LanguageId`)
    REFERENCES `cms`.`piranha_languages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_mediafolders`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_mediafolders` (
  `Id` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  `Description` VARCHAR(512) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_media`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_media` (
  `Id` CHAR(36) NOT NULL,
  `ContentType` VARCHAR(256) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `Filename` VARCHAR(128) NOT NULL,
  `FolderId` CHAR(36) NULL DEFAULT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `PublicUrl` LONGTEXT NULL DEFAULT NULL,
  `Size` BIGINT(20) NOT NULL,
  `Type` INT(11) NOT NULL,
  `Height` INT(11) NULL DEFAULT NULL,
  `Width` INT(11) NULL DEFAULT NULL,
  `AltText` VARCHAR(128) NULL DEFAULT NULL,
  `Description` VARCHAR(512) NULL DEFAULT NULL,
  `Properties` LONGTEXT NULL DEFAULT NULL,
  `Title` VARCHAR(128) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_Media_FolderId` (`FolderId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Media_Piranha_MediaFolders_FolderId`
    FOREIGN KEY (`FolderId`)
    REFERENCES `cms`.`piranha_mediafolders` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_mediaversions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_mediaversions` (
  `Id` CHAR(36) NOT NULL,
  `Height` INT(11) NULL DEFAULT NULL,
  `MediaId` CHAR(36) NOT NULL,
  `Size` BIGINT(20) NOT NULL,
  `Width` INT(11) NOT NULL,
  `FileExtension` VARCHAR(8) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_MediaVersions_MediaId_Width_Height` (`MediaId` ASC, `Width` ASC, `Height` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_MediaVersions_Piranha_Media_MediaId`
    FOREIGN KEY (`MediaId`)
    REFERENCES `cms`.`piranha_media` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pageblocks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pageblocks` (
  `Id` CHAR(36) NOT NULL,
  `BlockId` CHAR(36) NOT NULL,
  `PageId` CHAR(36) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_PageBlocks_PageId_SortOrder` (`PageId` ASC, `SortOrder` ASC) VISIBLE,
  INDEX `IX_Piranha_PageBlocks_BlockId` (`BlockId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PageBlocks_Piranha_Blocks_BlockId`
    FOREIGN KEY (`BlockId`)
    REFERENCES `cms`.`piranha_blocks` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_PageBlocks_Piranha_Pages_PageId`
    FOREIGN KEY (`PageId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pagecomments`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pagecomments` (
  `Id` CHAR(36) NOT NULL,
  `UserId` LONGTEXT NULL DEFAULT NULL,
  `Author` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(128) NOT NULL,
  `Url` VARCHAR(256) NULL DEFAULT NULL,
  `IsApproved` TINYINT(1) NOT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `PageId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PageComments_PageId` (`PageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PageComments_Piranha_Pages_PageId`
    FOREIGN KEY (`PageId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pagefields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pagefields` (
  `Id` CHAR(36) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `PageId` CHAR(36) NOT NULL,
  `RegionId` VARCHAR(64) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PageFields_PageId_RegionId_FieldId_SortOrder` (`PageId` ASC, `RegionId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PageFields_Piranha_Pages_PageId`
    FOREIGN KEY (`PageId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pagepermissions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pagepermissions` (
  `PageId` CHAR(36) NOT NULL,
  `Permission` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`PageId`, `Permission`),
  CONSTRAINT `FK_Piranha_PagePermissions_Piranha_Pages_PageId`
    FOREIGN KEY (`PageId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_pagerevisions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_pagerevisions` (
  `Id` CHAR(36) NOT NULL,
  `Data` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `PageId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PageRevisions_PageId` (`PageId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PageRevisions_Piranha_Pages_PageId`
    FOREIGN KEY (`PageId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_params`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_params` (
  `Id` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `Description` VARCHAR(256) NULL DEFAULT NULL,
  `Key` VARCHAR(64) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Params_Key` (`Key` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_posttypes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_posttypes` (
  `Id` VARCHAR(64) NOT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `CLRType` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_posts`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_posts` (
  `Id` CHAR(36) NOT NULL,
  `BlogId` CHAR(36) NOT NULL,
  `CategoryId` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `MetaDescription` VARCHAR(256) NULL DEFAULT NULL,
  `MetaKeywords` VARCHAR(128) NULL DEFAULT NULL,
  `PostTypeId` VARCHAR(64) NOT NULL,
  `Published` DATETIME(6) NULL DEFAULT NULL,
  `RedirectType` INT(11) NOT NULL,
  `RedirectUrl` VARCHAR(256) NULL DEFAULT NULL,
  `Route` VARCHAR(256) NULL DEFAULT NULL,
  `Slug` VARCHAR(128) NOT NULL,
  `Title` VARCHAR(128) NOT NULL,
  `CloseCommentsAfterDays` INT(11) NOT NULL DEFAULT '0',
  `EnableComments` TINYINT(1) NOT NULL DEFAULT '0',
  `Excerpt` LONGTEXT NULL DEFAULT NULL,
  `PrimaryImageId` CHAR(36) NULL DEFAULT NULL,
  `MetaTitle` VARCHAR(128) NULL DEFAULT NULL,
  `OgDescription` VARCHAR(256) NULL DEFAULT NULL,
  `OgImageId` CHAR(36) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `OgTitle` VARCHAR(128) NULL DEFAULT NULL,
  `MetaFollow` TINYINT(1) NULL DEFAULT '1',
  `MetaIndex` TINYINT(1) NULL DEFAULT '1',
  `MetaPriority` DOUBLE NOT NULL DEFAULT '0.5',
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Posts_BlogId_Slug` (`BlogId` ASC, `Slug` ASC) VISIBLE,
  INDEX `IX_Piranha_Posts_CategoryId` (`CategoryId` ASC) VISIBLE,
  INDEX `IX_Piranha_Posts_PostTypeId` (`PostTypeId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Posts_Piranha_Categories_CategoryId`
    FOREIGN KEY (`CategoryId`)
    REFERENCES `cms`.`piranha_categories` (`Id`),
  CONSTRAINT `FK_Piranha_Posts_Piranha_Pages_BlogId`
    FOREIGN KEY (`BlogId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_Posts_Piranha_PostTypes_PostTypeId`
    FOREIGN KEY (`PostTypeId`)
    REFERENCES `cms`.`piranha_posttypes` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_postblocks`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_postblocks` (
  `Id` CHAR(36) NOT NULL,
  `BlockId` CHAR(36) NOT NULL,
  `PostId` CHAR(36) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `ParentId` CHAR(36) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_PostBlocks_PostId_SortOrder` (`PostId` ASC, `SortOrder` ASC) VISIBLE,
  INDEX `IX_Piranha_PostBlocks_BlockId` (`BlockId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PostBlocks_Piranha_Blocks_BlockId`
    FOREIGN KEY (`BlockId`)
    REFERENCES `cms`.`piranha_blocks` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_PostBlocks_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_postcomments`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_postcomments` (
  `Id` CHAR(36) NOT NULL,
  `UserId` VARCHAR(128) NULL DEFAULT NULL,
  `Author` VARCHAR(128) NOT NULL,
  `Email` VARCHAR(128) NOT NULL,
  `Url` VARCHAR(256) NULL DEFAULT NULL,
  `IsApproved` TINYINT(1) NOT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `PostId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PostComments_PostId` (`PostId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PostComments_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_postfields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_postfields` (
  `Id` CHAR(36) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `PostId` CHAR(36) NOT NULL,
  `RegionId` VARCHAR(64) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PostFields_PostId_RegionId_FieldId_SortOrder` (`PostId` ASC, `RegionId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PostFields_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_postpermissions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_postpermissions` (
  `PostId` CHAR(36) NOT NULL,
  `Permission` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`PostId`, `Permission`),
  CONSTRAINT `FK_Piranha_PostPermissions_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_postrevisions`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_postrevisions` (
  `Id` CHAR(36) NOT NULL,
  `Data` LONGTEXT NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `PostId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_PostRevisions_PostId` (`PostId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PostRevisions_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_tags`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_tags` (
  `Id` CHAR(36) NOT NULL,
  `BlogId` CHAR(36) NOT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  `Slug` VARCHAR(64) NOT NULL,
  `Title` VARCHAR(64) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_Piranha_Tags_BlogId_Slug` (`BlogId` ASC, `Slug` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_Tags_Piranha_Pages_BlogId`
    FOREIGN KEY (`BlogId`)
    REFERENCES `cms`.`piranha_pages` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_posttags`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_posttags` (
  `PostId` CHAR(36) NOT NULL,
  `TagId` CHAR(36) NOT NULL,
  PRIMARY KEY (`PostId`, `TagId`),
  INDEX `IX_Piranha_PostTags_TagId` (`TagId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_PostTags_Piranha_Posts_PostId`
    FOREIGN KEY (`PostId`)
    REFERENCES `cms`.`piranha_posts` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_PostTags_Piranha_Tags_TagId`
    FOREIGN KEY (`TagId`)
    REFERENCES `cms`.`piranha_tags` (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_roles` (
  `Id` CHAR(36) NOT NULL,
  `ConcurrencyStamp` LONGTEXT NULL DEFAULT NULL,
  `Name` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `RoleNameIndex` (`NormalizedName` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_roleclaims`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_roleclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` LONGTEXT NULL DEFAULT NULL,
  `ClaimValue` LONGTEXT NULL DEFAULT NULL,
  `RoleId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_RoleClaims_RoleId` (`RoleId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_RoleClaims_Piranha_Roles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `cms`.`piranha_roles` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 56
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_sitefields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_sitefields` (
  `Id` CHAR(36) NOT NULL,
  `CLRType` VARCHAR(256) NOT NULL,
  `FieldId` VARCHAR(64) NOT NULL,
  `RegionId` VARCHAR(64) NOT NULL,
  `SiteId` CHAR(36) NOT NULL,
  `SortOrder` INT(11) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_SiteFields_SiteId_RegionId_FieldId_SortOrder` (`SiteId` ASC, `RegionId` ASC, `FieldId` ASC, `SortOrder` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_SiteFields_Piranha_Sites_SiteId`
    FOREIGN KEY (`SiteId`)
    REFERENCES `cms`.`piranha_sites` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_sitetypes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_sitetypes` (
  `Id` VARCHAR(64) NOT NULL,
  `Body` LONGTEXT NULL DEFAULT NULL,
  `CLRType` VARCHAR(256) NULL DEFAULT NULL,
  `Created` DATETIME(6) NOT NULL,
  `LastModified` DATETIME(6) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_users` (
  `Id` CHAR(36) NOT NULL,
  `AccessFailedCount` INT(11) NOT NULL,
  `ConcurrencyStamp` LONGTEXT NULL DEFAULT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `EmailConfirmed` TINYINT(1) NOT NULL,
  `LockoutEnabled` TINYINT(1) NOT NULL,
  `LockoutEnd` DATETIME(6) NULL DEFAULT NULL,
  `NormalizedEmail` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedUserName` VARCHAR(256) NULL DEFAULT NULL,
  `PasswordHash` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumber` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumberConfirmed` TINYINT(1) NOT NULL,
  `SecurityStamp` LONGTEXT NULL DEFAULT NULL,
  `TwoFactorEnabled` TINYINT(1) NOT NULL,
  `UserName` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserNameIndex` (`NormalizedUserName` ASC) VISIBLE,
  INDEX `EmailIndex` (`NormalizedEmail` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_userclaims`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_userclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` LONGTEXT NULL DEFAULT NULL,
  `ClaimValue` LONGTEXT NULL DEFAULT NULL,
  `UserId` CHAR(36) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_Piranha_UserClaims_UserId` (`UserId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_UserClaims_Piranha_Users_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `cms`.`piranha_users` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_userlogins`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_userlogins` (
  `LoginProvider` VARCHAR(255) NOT NULL,
  `ProviderKey` VARCHAR(255) NOT NULL,
  `ProviderDisplayName` LONGTEXT NULL DEFAULT NULL,
  `UserId` CHAR(36) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`),
  INDEX `IX_Piranha_UserLogins_UserId` (`UserId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_UserLogins_Piranha_Users_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `cms`.`piranha_users` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_userroles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_userroles` (
  `UserId` CHAR(36) NOT NULL,
  `RoleId` CHAR(36) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `IX_Piranha_UserRoles_RoleId` (`RoleId` ASC) VISIBLE,
  CONSTRAINT `FK_Piranha_UserRoles_Piranha_Roles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `cms`.`piranha_roles` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_Piranha_UserRoles_Piranha_Users_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `cms`.`piranha_users` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `cms`.`piranha_usertokens`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `cms`.`piranha_usertokens` (
  `UserId` CHAR(36) NOT NULL,
  `LoginProvider` VARCHAR(255) NOT NULL,
  `Name` VARCHAR(255) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
  CONSTRAINT `FK_Piranha_UserTokens_Piranha_Users_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `cms`.`piranha_users` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
