import { Route, Routes } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import UserApi from "./api/UserApi";
import { useContext, useEffect, useState } from "react";
import { Context } from "./main";

export default function App() {

    const stores = useContext(Context);
    const [isLoaded, setIsLoaded] = useState(false);
  
    useEffect(() => {
      if (!window.location.pathname.includes("/signin"))
      {
        if (localStorage.getItem('token') == null) {
          stores?.userStore.setUser(null);
          window.location.pathname = "/signin";
          return;
        }
        UserApi.auth()
        .then((res: any) => {
          stores?.userStore.setUser(res);
          setIsLoaded(true);
        })
        .catch(() => UserApi.signOut());
      }
      else 
      setIsLoaded(true);
  
      
    }, [])
    return(
      <>
      {isLoaded ?
       <Routes>
       {AppRoutes.map((item, index) => <Route key={index} {...item}/>)}
     </Routes>
     : <></>
      }
       
      </>
    )
  
  }