## 应用程序和企业应用程序
## 管理员授权和撤销
https://login.microsoftonline.com/{tenant}/v2.0/adminconsent?
client_id=6731de76-14a6-49ae-97bc-6eba6914391e
&state=12345
&redirect_uri=http://localhost/myapp/permissions
&scope=
https://graph.microsoft.com/calendars.read
https://graph.microsoft.com/mail.send
## 国内版
## 多租户
## 刷新token
## 用户撤销权限
## RBAC
## 自定义token属性
## 令牌生存期 https://docs.microsoft.com/zh-cn/azure/active-directory/develop/configure-token-lifetimes
## 两套SDK, MSAL，MicrosoftGraph
两套Client，public和confidential