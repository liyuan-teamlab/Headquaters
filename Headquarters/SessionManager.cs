﻿using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace Headquarters
{
    class SessionManager
    {
        #region Singleton
        public static SessionManager Instance { get; } = new SessionManager();

        private SessionManager()
        {
        }

        #endregion

        PowerShellScript createSession = new PowerShellScript("CreateSession",
@"
param($ComputerName,$cred)
New-PSSession -ComputerName $ComputerName -Credential $cred
");



        public PowerShellScript.Result CreateSession(Runspace rs, string ipAddress)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("ComputerName", ipAddress);
            dic.Add("cred", CreateCredential());

            return createSession.Invoke(rs, dic);
        }

        PSCredential CreateCredential()
        {
            var userName = ParameterManager.Instance.userName;
            var passwordStr = ParameterManager.Instance.userPassword;

            SecureString password = new SecureString();
            passwordStr?.ToList().ForEach(c => password.AppendChar(c));

            return string.IsNullOrEmpty(userName) ? PSCredential.Empty : new PSCredential(userName, password);
        }
    }
}