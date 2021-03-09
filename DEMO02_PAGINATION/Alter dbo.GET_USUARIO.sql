USE [BD_SEGURIDAD]
GO

/****** Objeto: SqlProcedure [dbo].[GET_USUARIO] Fecha del script: 17/08/2019 19:24:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE GET_USUARIO
(
	@OffsetValue int,
	@PagingSize int
)
AS
BEGIN

	SELECT Id, Usuario, Contrasena, Intentos, NivelSeg, FechaReg FROM USUARIO ORDER BY Id
	OFFSET @OffsetValue ROWS FETCH NEXT @PagingSize ROWS ONLY;

	Select count(Id) as TotalRows from USUARIO
END
