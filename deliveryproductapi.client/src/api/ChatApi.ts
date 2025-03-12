import axios from "axios";
import { ChatType } from "../types";
import { API_BASE, getToken } from "../consts";

class ChatApi {
    async getAll() : Promise<ChatType[]> {
        let result : ChatType[] = [];
        try {
            await axios.get(`${API_BASE}/api/chat/get_all`, {
                headers: {
                    Authorization: getToken()
                }
            })
            .then(res => result = res.data)
            .catch();
        } catch (ex) {}
        return result;
    }

    async getId(orderId: number) : Promise<number> {
        let result = 0;
        try {
            await axios.get(`${API_BASE}/api/chat/get_id?orderId=${orderId}`, {
                headers: {Authorization: getToken()}
            })
            .then(res => result = parseInt(res.data ?? '0'))
            .catch()
        } catch (ex) {}

        return result;
    }

    async add(orderId: number) {
        try {
            await axios.post(`${API_BASE}/api/chat/add/${orderId}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
            .then()
            .catch();
        } catch(ex) {}
    }

    async uploadImage(file: File, chatId: number) : Promise<string> {
        let result = "";
        const formData = new FormData();
        formData.append("file", file);
        formData.append("chatId", chatId.toString());

        try {
            await axios.post(`${API_BASE}/api/chat/upload_image`, formData, {
                headers: {
                    Authorization: getToken(),
                    "Content-Type": "multipart/form-data"
                }
            })
            .then(res => result = res.data)
            .catch();
        } catch (ex) {
            console.log(ex);
        }
        return result;
    }
}

export default new ChatApi();