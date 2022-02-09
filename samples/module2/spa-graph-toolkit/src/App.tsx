import "./styles.css";
import {
  Login,
  Providers,
  ProviderState,
  Agenda,
  FileList,
  People,
  Get,
  MgtTemplateProps
} from "@microsoft/mgt-react";
import { useEffect, useState } from "react";
import { Message } from "@microsoft/microsoft-graph-types";

function useIsSignIn(): [boolean] {
  const [isSignIn, setIsSignIn] = useState(false);
  useEffect(() => {
    const updateState = () => {
      const provider = Providers.globalProvider;
      setIsSignIn(provider && provider.state === ProviderState.SignedIn);
    };

    Providers.onProviderUpdated(updateState);
    updateState();
    return () => Providers.removeProviderUpdatedListener(updateState);
  }, []);

  return [isSignIn];
}

const EmailItem = (props: MgtTemplateProps) => {
  const message = props.dataContext as Message;
  return <div>{message.subject}</div>;
};

export default function App() {
  const [isSignIn] = useIsSignIn();

  return (
    <div className="App">
      <header>
        <Login />
      </header>
      <div>{isSignIn}</div>
      <div>
        {isSignIn && (
          <>
            <Get scopes={["Mail.Read"]} resource="/me/messages">
              <EmailItem template="value" />
            </Get>
            <Agenda />
            <FileList />
            <People />
          </>
        )}
      </div>
    </div>
  );
}
