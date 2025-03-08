import axios from "axios";
import { API_BASE, getToken } from "../consts";
import { UserType } from "../types";

class UserApi {

    async signIn(password: string, login?: string | null, emailAddress?: string | null): Promise<string> {
        let resultMessage = "";

        let token = getToken();

        if (token != null)
            return "Вы уже авторизованы";

        await axios.post(`${API_BASE}/api/user/signinadmin`, {
            login, password, emailAddress
        })
            .then((res: any) => {
                if (res.data) {
                    localStorage.setItem('token', res.data.token);
                    window.location.pathname = '/';
                }
            })
            .catch(err => resultMessage = err.response?.data ?? "Ошибка сервера")

        return resultMessage;
    }

    async auth(): Promise<UserType | null> {
        let result: UserType | null = null;
        let token = getToken();
        if (token == null) return null;

        await axios.get(`${API_BASE}/api/user/auth`, {
            headers: {
                Authorization: token
            }
        })
            .then((res: any) => result = res.data)
            .catch()

        return result;
    }

    signOut() {
        localStorage.removeItem('token');
        if (!window.location.pathname.includes('/signin'))
            window.location.reload();
    }
}

export default new UserApi();