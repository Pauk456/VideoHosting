import React, {useState} from 'react';

const RegistrationForm = ({ onClose }) => {
    const [registration, setRegistration] = useState(false)
    const [error, setError] = useState(false)


    const handleRegisterClick = () => {
        setRegistration(true)
        setError(false);

    }

    const handleLoginClick = () => {
        setRegistration(false);
        setError(true);
    }

    return (
        <div className="registration-form">
            <button className="close-button" onClick={onClose}>×</button>
            <p className="registration-form-title-text ">Авторизация</p>
            <form className="registration-form-plain-text registration-form-body">
                {error && <p className={"login-error-text"} style={{color: "red"}}>Неверный логин или пароль</p>}
                <input className={"regular-font-weight"} type="text" placeholder="Логин" />
                <input className={"regular-font-weight"} type="password" placeholder="Пароль" />
                {registration && <input className={"regular-font-weight"} type="password" placeholder="Подтверждение пароля" />}

                {/*Сделать кнопки на отдельных строчках*/}
                <div className={"auth-buttons-container"}>
                    <button  onClick={handleLoginClick} className={"hover-effect"} type="button">Войти</button>
                    <button  onClick={handleRegisterClick} className={"hover-effect"} type="button">Зарегистрироваться</button>
                </div>
            </form>
        </div>
    );
};

export default RegistrationForm;