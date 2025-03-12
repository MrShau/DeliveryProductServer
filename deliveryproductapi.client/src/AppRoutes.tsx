import Category from "./pages/Category";
import Chats from "./pages/Chats";
import Home from "./pages/Home";
import Orders from "./pages/Orders";
import SignIn from "./pages/SignIn";

export default [
    {
        path: "/signin",
        element: <SignIn />
    },
    {
        path: "/",
        element: <Home />
    },
    {
        path: "/category/:id",
        element: <Category />
    },
    {
        path: "/orders",
        element: <Orders />
    },
    {
        path: "/chats/:id?",
        element: <Chats />
    }
]