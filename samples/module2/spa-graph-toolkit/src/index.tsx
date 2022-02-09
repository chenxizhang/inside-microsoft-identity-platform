import { render } from "react-dom";
import { Providers } from "@microsoft/mgt-element";
import { Msal2Provider } from "@microsoft/mgt-msal2-provider";

import App from "./App";

Providers.globalProvider = new Msal2Provider({
  clientId: "397b5acf-6a97-4307-a87e-b8e691e54d41",
  authority:
    "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd",
  loginType: 0,
  scopes: ["User.Read", "Mail.Read"]
});

const rootElement = document.getElementById("root");
render(<App />, rootElement);
