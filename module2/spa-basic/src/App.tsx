import "./styles.css";
import { useMsal, useIsAuthenticated } from "@azure/msal-react";
import { IPublicClientApplication } from "@azure/msal-browser";
import { Client } from "@microsoft/microsoft-graph-client";
import { useState } from "react";
import { Message, User } from "@microsoft/microsoft-graph-types";

function handleLogin(instance: IPublicClientApplication): void {
  instance
    .loginPopup()
    .then((v) => instance.setActiveAccount(v.account))
    .catch((reason) => alert(reason));
}

function handleLogout(instance: IPublicClientApplication): void {
  instance.logoutPopup();
}

const getToken = async (
  instance: IPublicClientApplication,
  scopes: string[]
): Promise<string> => {
  const request = { scopes: scopes };
  try {
    const result = await instance.acquireTokenSilent(request);
    return result.accessToken;
  } catch (error) {
    try {
      const result = await instance.acquireTokenPopup(request);
      return result.accessToken;
    } catch (error) {
      throw error;
    }
  }
};

async function getUserProfile(token: string) {
  const client = Client.init({
    authProvider: (done) => done(undefined, token)
  });
  return client.api("/me").get();
}

async function getEmailMessages(token: string) {
  const client = Client.init({
    authProvider: (done) => done(undefined, token)
  });
  return client.api("/me/messages").get();
}

export default function App() {
  const isAuthenticated = useIsAuthenticated();
  const { instance, accounts } = useMsal();
  const [messages, setMessages] = useState<Message[]>();
  const [user, setUser] = useState<User>();

  if (isAuthenticated && accounts && accounts.length > 0)
    instance.setActiveAccount(accounts[0]);

  return (
    <div className="App">
      {isAuthenticated && accounts && accounts.length > 0 && (
        <>
          您好,{accounts[0].name}!
          <p>
            <button
              onClick={() =>
                getToken(instance, ["User.Read"])
                  .then((x) => getUserProfile(x).then((v) => setUser(v)))
                  .catch((reason) => console.log(reason))
              }
            >
              我的信息
            </button>{" "}
            <button
              onClick={() =>
                getToken(instance, ["Mail.Read"])
                  .then((x) => getEmailMessages(x).then((v) => v.value))
                  .then((v) => setMessages(v))
                  .catch((reason) => console.log(reason))
              }
            >
              我的邮件
            </button>{" "}
            <button onClick={() => handleLogout(instance)}>注销</button>
          </p>
          <p>
            {user && (
              <span>
                {user.userPrincipalName},{user.givenName},{user.id}
              </span>
            )}
          </p>
          <p>
            {messages && messages.map((m) => <li key={m.id}>{m.subject}</li>)}
          </p>
        </>
      )}

      {!isAuthenticated && (
        <>
          <button onClick={() => handleLogin(instance)}>请登录</button>
        </>
      )}
    </div>
  );
}
