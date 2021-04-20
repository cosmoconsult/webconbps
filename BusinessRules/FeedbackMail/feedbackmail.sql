select 
  'mailto:' + 
    isnull(
        -- Get mail of process supervisor if one exists
        ( select COS_AD_mail
          from CacheOrganizationStructure
          where COS_BpsID = (
                  select Top 1 [dbo].ClearWFElemIDAdv([DEF_Supervisor]) as SupervisorId
                  from [WFDefinitions]
                  where DEF_Id = {DEF_ID})
         )
        ,isnull(
            -- Get custom application supervisor mail if one exists
            (  select [APP_CustomSupervisorMail]
               from [WFApplications]
               where APP_Id = {APP_ID}
           )
        ,isnull(
            -- Get mail of application supervisor if one exists
            (    select COS_AD_mail
                 from CacheOrganizationStructure
                where COS_BpsID = (
                      select Top 1 [dbo].ClearWFElemIDAdv([APP_Supervisor]) as SupervisorId
                      from [WFApplications]
                      where APP_Id = {APP_ID})
            )
            -- Fallback if no supervisors have been defined
            ,'Undefined'
        ) -- Closing application supervisor and fallback
    ) -- closing custom application supervisor
) -- closing process supervisor
+'?subject=Feedback {APP_Name}:&body=Dear supervisor,'
+'%0D%0D%0Dtype your text and provide screenshots if applicable.%0D%0D%0D'
+'Best regards,%0D'
+'{N:CURRENTUSER}%0D'
+'______________________________________________%0d'
+'System information%0D'
+'Application: {APP_Name}%0D'
+'Workflow: {WF_Name}%0D'
+'Form Type: {DTYPE_Name}%0D'
+'Step: {STP_Name}%0D'
+'Instance link: {EGV:3}/db/{DBID}/app/{APP_ID}/element/{WFD_ID}/form %0D'
+'Current User: {CURRENTUSER}%0D'
+'Date time: {CURRENTDATETIME}%0D'
+'Browser language: {USERLAN}'