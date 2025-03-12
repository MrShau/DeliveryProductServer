import { makeAutoObservable } from "mobx";
import { UserType } from "../types"

export default class UserStore {
    user: UserType | null = null;
    isAuth: boolean = this.user != null;

    constructor() {
        makeAutoObservable(this);
    }

    public setUser(value: UserType | null) {
        this.user = value;
    }
}

