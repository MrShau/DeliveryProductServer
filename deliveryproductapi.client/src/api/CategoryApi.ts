import axios from "axios";
import { API_BASE, getToken } from "../consts";
import { CategoryType, ProductType } from "../types";

class CategoryApi {

    async get(id: number) : Promise<CategoryType | null> {
        let result: CategoryType | null = null;
        try {
            await axios.get(`${API_BASE}/api/category/get?id=${id}`)
                .then(res => {
                    result = res.data
                })
                .catch();
        }
        catch (ex) {
            console.log(ex);
        }
        return result;
    }

    async getAll() : Promise<CategoryType[]> {
        let categories: CategoryType[] = [];
        try {
            await axios.get(`${API_BASE}/api/category/get_all`)
                .then(res => {
                    categories = res.data
                })
                .catch();
        }
        catch (ex) {
            console.log(ex);
        }
        return categories;
    }

    async add(name: string) {
        try {
            await axios.post(`${API_BASE}/api/category/add`, {name}, {
                headers: {Authorization: getToken()}
            })
                .then(() => alert("Категория успешно добавлена"))
                .catch(() => alert("Не удалось добавить категорию"));
        }
        catch (ex) {console.log(ex)}
    }

    async delete(id: number) {
        try {
            await axios.delete(`${API_BASE}/api/category/delete?id=${id}`, {
                headers: {Authorization: getToken()}
            })
            .then(() => alert("Категория успешно была удалена"))
            .catch(() => alert("Не удалось удалить категорию, ошибка"))
        } catch (ex) {console.log(ex)}
    }

    async addProduct(categoryId: number, title: string, description: string, imageFile: File, price: number, weight: number, weightUnit: string, count: number) {
        const formData = new FormData();
        formData.append("categoryId", categoryId.toString());
        formData.append("title", title);
        formData.append("description", description);
        formData.append("imageFile", imageFile);
        formData.append("price", price.toString());
        formData.append("weight", weight.toString());
        formData.append("weightUnit", weightUnit);
        formData.append("count", count.toString());

        try {
            await axios.post(`${API_BASE}/api/category/add_product`, formData, {
                headers: {
                    Authorization: getToken(),
                    "Content-Type": "multipart/form-data"
                }
            })
            .then(() => alert("Продукт успешно добавлен"))
            .catch(() => alert("Не удалось добавить продукт"));
        } catch (ex) {
            console.log(ex);
        }
    }

    async getProducts(categoryId: number) : Promise<ProductType[]> {
        let result : ProductType[] = [];
        try {
            await axios.get(`${API_BASE}/api/category/get_products?categoryId=${categoryId}`)
            .then((res) => result = res.data)
            .catch((err) => console.log(err));
        } catch (ex) {
            console.log(ex);
        }
        return result;
    }
}

export default new CategoryApi();