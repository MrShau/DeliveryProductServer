import axios from "axios";
import { OrderType } from "../types";
import { API_BASE, getToken } from "../consts";

class OrderApi {
    async getWaitConfirmations() : Promise<OrderType[]> {
        let result: OrderType[] = [];

        try {
            await axios.get(`${API_BASE}/api/order/get_wait_confirmations`, {
                headers: {Authorization: getToken()}
            })
            .then(res => result = res.data)
            .catch()
        } catch(ex) {}

        return result;
    }

    async get(id: number) : Promise<OrderType | null> {
        let result : OrderType | null = null;

        try {
            await axios.get(`${API_BASE}/api/order/get?id=${id}`, {
                headers: {Authorization: getToken()}
            })
            .then(res => result = res.data)
            .catch();
        } catch (ex) {}

        return result;
    }

    async getActives() : Promise<OrderType[]> {
        let result: OrderType[] = [];

        try {
            await axios.get(`${API_BASE}/api/order/get_actives`, {
                headers: {Authorization: getToken()}
            })
            .then(res => result = res.data)
            .catch()
        } catch(ex) {}

        return result;
    }

    async getCompleted() : Promise<OrderType[]> {
        let result: OrderType[] = [];

        try {
            await axios.get(`${API_BASE}/api/order/get_completed`, {
                headers: {Authorization: getToken()}
            })
            .then(res => result = res.data)
            .catch()
        } catch(ex) {}

        return result;
    }

    async confirm(id: number) {
        try {
            await axios.post(`${API_BASE}/api/order/confirm/${id}`, {}, {
                headers: {Authorization: getToken()}
            })
            .then(() => {
                alert("Заказ подтвержден !");
                window.location.reload();
            })
            .catch(() => alert("Не удалось подтвердить заказ !"))
        } catch(ex) {}
    }
}

export default new OrderApi();