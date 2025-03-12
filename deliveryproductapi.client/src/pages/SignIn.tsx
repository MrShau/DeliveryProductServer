import { Button, Form } from "react-bootstrap";
import { FaCircleExclamation, FaLock, FaUser, FaUserAstronaut } from "react-icons/fa6";
import { PASSWORD_MAX, PASSWORD_MIN } from "../consts";
import { useState } from "react";

import './SignIn.css'
import UserApi from "../api/UserApi";
import CategoryApi from "../api/CategoryApi";

export default function SignIn() {

    const [errorMsg, setErrorMsg] = useState("");
    const [emailOrLogin, setEmailOrLogin] = useState("");
    const [password, setPassword] = useState("");

    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    const loginPattern = /^[a-zA-Z0-9_]{3,}$/;
    CategoryApi.getAll().then(res => console.log(res))
    const signUp = async (e: any) => {
        e.preventDefault();

        if (localStorage.getItem('token') != null)
            return setErrorMsg(`Вы уже авторизованы !`);
        
        if (!(emailPattern.test(emailOrLogin) || loginPattern.test(emailOrLogin)))
            return setErrorMsg(`Введите корректный логин или почту`)
        else if (password.length < PASSWORD_MIN || password.length > PASSWORD_MAX)
            return setErrorMsg(`Неправильный пароль`)
        
        setErrorMsg('');

        const emailAddress = emailPattern.test(emailOrLogin) ? emailOrLogin : null;
        const login = loginPattern.test(emailOrLogin) ? emailOrLogin : null;

        setErrorMsg(await UserApi.signIn(password, login, emailAddress));
    }


    return (
        <>
            <div className="d-flex align-items-center vh-100">
                <div className='form-auth mx-auto px-3 py-3 align-items-center border shadow col-12 col-md-6 col-xl-4 rounded-4'>
                    <div className="text-center" style={{ fontSize: "58px" }}>
                        <FaUserAstronaut />
                    </div>
                    <h3 className='title-nunito text-center my-4'>Авторизация</h3>

                    <form className='mx-lg-5 mt-4 mb-3 pt-2'>
                        <div className='d-flex mb-3 border'>
                            <div className='text-center d-flex align-items-center px-3 py-3'>
                                <FaUser />
                            </div>
                            <Form.Control 
                            className='signup-input w-100 px-3 rounded-0' 
                            placeholder='Логин / Email' onChange={e => setEmailOrLogin(e.target.value)} />
                        </div>
                        <div className='d-flex mb-3 border'>
                            <div className='text-center d-flex align-items-center px-3 py-3'>
                                <FaLock />
                            </div>
                            <Form.Control className='signup-input w-100 px-3 rounded-0' placeholder='Пароль' onChange={e => setPassword(e.target.value)} />
                        </div>

                        <div className='text-danger mx-2 d-block my-2'>
                            {errorMsg.length > 0 ? <FaCircleExclamation className="me-1" /> : null} {errorMsg}
                        </div>

                        <Button className='w-100 mt-3 mb-3 rounded-0 py-2 btn-lg' onClick={signUp}>Продолжить</Button>
                    </form>
                </div>
            </div>
        </>
    )
}