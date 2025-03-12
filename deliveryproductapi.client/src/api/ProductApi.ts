import axios from "axios";
import { ProductType } from "../types";
import { API_BASE, getToken } from "../consts";

class ProductApi {
    async get(id: number) : Promise<ProductType | null> {
        let result: ProductType | null = null;

        try {
            await axios.get(`${API_BASE}/api/product/get?id=${id}`)
                .then(res => { result = res.data})
                .catch()
        } catch(ex) {console.log(ex)}

        return result;
    }

    async updateTitle(id: number, title: string) {
        try {
            await axios.patch(`${API_BASE}/api/product/update_title/${id}/${title}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
                .then(() => {})
                .catch(() => {alert("Не удалось изменить название, ошибка")})
        } catch(ex) {console.log(ex)}
    }

    async updateDescription(id: number, description: string) {
        try {
            await axios.patch(`${API_BASE}/api/product/update_description/${id}/${description}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
                .then(() => {})
                .catch(() => {alert("Не удалось изменить описание, ошибка")})
        } catch(ex) {console.log(ex)}
    }

    async updatePrice(id: number, price: number) {
        try {
            await axios.patch(`${API_BASE}/api/product/update_price/${id}/${price}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
                .then(() => {})
                .catch(() => {alert("Не удалось изменить цену, ошибка")})
        } catch(ex) {console.log(ex)}
    }

    async updateCategory(id: number, categoryId: number) {
        try {
            await axios.patch(`${API_BASE}/api/product/update_category/${id}/${categoryId}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
                .then(() => {})
                .catch(() => {alert("Не удалось изменить категорию, ошибка")})
        } catch(ex) {console.log(ex)}
    }

    async updateCount(id: number, count: number) {
        try {
            await axios.patch(`${API_BASE}/api/product/update_count/${id}/${count}`, {}, {
                headers: {
                    Authorization: getToken()
                }
            })
                .then(() => {})
                .catch(() => {alert("Не удалось изменить количество, ошибка")})
        } catch(ex) {console.log(ex)}
    }

    async updateImage(id: number, imageFile: File) {
        let formData = new FormData()
        formData.append("id", id.toString())
        formData.append("imageFile", imageFile)
        try {
            await axios.patch(`${API_BASE}/api/product/update_image`, formData, {
                headers: {
                    Authorization: getToken(),
                    "Content-Type": "multipart/form-data"
                }
            })
            .then()
            .catch(() => alert("Не удалось изменить изображение, ошибка"));
        }
        catch (ex) {console.log(ex)}
    }

    async delete(id: number) {
        try {
            await axios.delete(`${API_BASE}/api/product/delete?id=${id}`, {
                headers: {Authorization: getToken()}
            })
            .then(() => alert("Продукт успешно была удалена"))
            .catch(() => alert("Не удалось удалить продукт, ошибка"))
        } catch (ex) {console.log(ex)}
    }
}

export default new ProductApi();