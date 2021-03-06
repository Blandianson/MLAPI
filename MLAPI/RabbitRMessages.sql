/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) 
      [ErrorMessage]
      ,[DateCreated]
	  ,[MessageId]
      ,[ExecutionId]
      ,[MessageBody]
      ,[Status]
      ,[DateUpdated]
      ,[SessionId]
      ,[OutputMessage]
      ,[HostName]
      ,[WarningMessage]
  FROM [HaloMessageClient].[Queue].[Message] ORDER BY DateCreated DESC