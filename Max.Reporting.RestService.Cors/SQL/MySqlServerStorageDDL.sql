
CREATE TABLE IF NOT EXISTS `tr_applock` (
  `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `tr_object` (
  `Id` varchar(255) NOT NULL,
  `Value` longblob NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `tr_set` (
  `Id` varchar(255) NOT NULL,
  `Member` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`,`Member`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `tr_string` (
  `Id` varchar(255) NOT NULL,
  `Value` varchar(4000) CHARACTER SET utf8mb4 NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


DROP PROCEDURE IF EXISTS sp_tr_AcquireLock;
CREATE PROCEDURE `sp_tr_AcquireLock`(
    p_Key varchar(255) /* = 0 */)
BEGIN
    INSERT INTO `tr_AppLock`
    VALUES (p_Key);
END;

DROP PROCEDURE IF EXISTS sp_tr_AddInSet;
CREATE PROCEDURE `sp_tr_AddInSet`(
    p_Key varchar(255) ,
    p_Value varchar(255))
BEGIN
	INSERT INTO tr_Set (ID,Member) values (p_Key,p_Value)
    on duplicate key update Member = p_Value;    
END;

DROP PROCEDURE IF EXISTS sp_tr_ClearAll;
CREATE PROCEDURE `sp_tr_ClearAll`()
BEGIN
    DELETE from `tr_Set`;
    DELETE from `tr_Object`;
    DELETE from `tr_String`;
END;

DROP PROCEDURE IF EXISTS sp_tr_Delete;
CREATE  PROCEDURE `sp_tr_Delete`(
    p_Key varchar(255) /* = 0 */)
BEGIN

    DELETE
    FROM `tr_String`
    WHERE `Id` = p_Key;

    DELETE
    FROM `tr_Object`
    WHERE `Id` = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_DeleteInSet;
CREATE PROCEDURE `sp_tr_DeleteInSet`(
    p_Key varchar(255),
    p_Value varchar(255),
    out p_return int)
BEGIN
    DELETE FROM `tr_Set` WHERE `Id` = p_Key AND `Member` = p_Value;
    set p_return= FOUND_ROWS();
END;

DROP PROCEDURE IF EXISTS sp_tr_DeleteLike;
CREATE  PROCEDURE `sp_tr_DeleteLike`(
    p_Key varchar(255))
BEGIN

    DELETE
    FROM `tr_String`
    WHERE `Id` LIKE CONCAT(p_Key , '%');

    DELETE
    FROM `tr_Object`
    WHERE `Id` LIKE CONCAT(p_Key , '%');

    DELETE
    FROM `tr_Set`
    WHERE `Id` LIKE CONCAT(p_Key , '%');
END;

DROP PROCEDURE IF EXISTS sp_tr_DeleteSet;
CREATE PROCEDURE `sp_tr_DeleteSet`(
    p_Key varchar(255) /* = 0 */)
BEGIN

    DELETE
    FROM `tr_Set`
    WHERE `Id` = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_ExistsInSet;
CREATE PROCEDURE `sp_tr_ExistsInSet`(
    p_Key varchar(255),
    p_Value varchar(255),
    out p_return int)
sp_lbl:

BEGIN

    IF EXISTS (SELECT 1 FROM `tr_Set` WHERE `Id` = p_Key AND `Member` = p_Value) THEN
        set p_return = 1;
    ELSE
        set p_return = 0;
    END IF;
END;

DROP PROCEDURE IF EXISTS sp_tr_GetBytes;
CREATE PROCEDURE `sp_tr_GetBytes`(
    p_Key varchar(255))
BEGIN

    SELECT Value
    FROM `tr_Object`
    WHERE Id = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_GetCountInSet;
CREATE PROCEDURE `sp_tr_GetCountInSet`(
    p_Key varchar(255))
BEGIN    
    SELECT COUNT(1)
    FROM `tr_Set`
    WHERE `Id` = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_GetMembersInSet;
CREATE PROCEDURE `sp_tr_GetMembersInSet`(
    p_Key varchar(255))
BEGIN

SELECT `Member`
FROM `tr_Set`
WHERE `Id` = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_GetString;
CREATE PROCEDURE `sp_tr_GetString`(
    IN p_Key varchar(255))
BEGIN

    SELECT Value
    FROM `tr_String`
    WHERE Id = p_Key;
END;

DROP PROCEDURE IF EXISTS sp_tr_SetObject;
CREATE PROCEDURE `sp_tr_SetObject`(
    p_Key varchar(255),
    p_Value longblob)
BEGIN
	INSERT INTO tr_Object (ID,Value) values (p_Key,p_Value)
    on duplicate key update Value = p_Value;  
END;

DROP PROCEDURE IF EXISTS sp_tr_SetString;
CREATE  PROCEDURE `sp_tr_SetString`(
    p_Key varchar(255),
    p_Value nvarchar(4000))
BEGIN
	INSERT INTO tr_String (ID,Value) values (p_Key,p_Value)
    on duplicate key update Value = p_Value;     
END;
