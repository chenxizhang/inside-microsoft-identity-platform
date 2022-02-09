import { render } from "react-dom";

import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";

import App from "./App";

const instance = new PublicClientApplication({
  auth: {
    clientId: "46abe5ec-ccf0-4805-a7b5-f622142b6854",
    authority:
      "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd"
  },
  cache: {
    cacheLocation: "sessionStorage"
  }
});

const rootElement = document.getElementById("root");
render(
  <MsalProvider instance={instance}>
    <App />
  </MsalProvider>,
  rootElement
);
