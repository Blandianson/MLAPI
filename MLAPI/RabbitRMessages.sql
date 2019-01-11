/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [MessageId]
      ,[ExecutionId]
      ,[MessageBody]
      ,[Status]
      ,[ErrorMessage]
      ,[DateCreated]
      ,[DateUpdated]
      ,[SessionId]
      ,[OutputMessage]
      ,[HostName]
      ,[WarningMessage]
  FROM [HaloMessageClient].[Queue].[Message] ORDER BY DateCreated DESC