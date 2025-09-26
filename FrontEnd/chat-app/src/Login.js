import React, { useState } from "react";
import axios from "axios";
import { useNavigate, Link } from 'react-router-dom';

function Login() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const Navigate = useNavigate();

    const handleLogin = (e) => {
        // try {
        e.preventDefault();
        const API_URL = "https://localhost:7246/api/Auth/Login";
        axios.post(API_URL, {
            username,
            email,
            password
            }, {
            headers: { "Content-Type": "application/json" }
            }
        ).then((e) => {
            // console.log(res)
            console.log(e)
            localStorage.setItem("jwt", e.data.accessToken);
            Navigate('/chat')

        }).catch(() => {
            // console.log(error)
            alert('Email or Password is incorrect!')
        })

    };
    //Email set in localStorage
    const setDataOnStorage = (chat_email) => {
        localStorage.setItem('chatEmail', chat_email)
    }

    return (

        <div className="row">
            <div className="col-md-4 m-auto mt-5 bg-white">
                <div className='p-4 text-center text-dark font-weight-bold mb-4 createdata'>
                    <h1>Login</h1>
                </div>
                <form onSubmit={handleLogin}>
                    <div className="form-group">
                        <label className='form-label font-weight-bold'>User Name:</label>
                        <input
                            type="text"
                            placeholder="Username"
                            onChange={(e) => setUsername(e.target.value)}
                            className="form-control mb-3"
                        />
                    </div>
                    <div className="form-group">
                        <label className='form-label font-weight-bold'>Email:</label>
                        <input
                            type="email"
                            placeholder="Useremail"
                            onChange={(e) => setEmail(e.target.value)}
                            className="form-control mb-3"
                        />
                    </div>
                    <div className="form-group">
                        <label className='form-label font-weight-bold'>Password:</label>
                        <input
                            type="password"
                            placeholder="Password"
                            onChange={(e) => setPassword(e.target.value)}
                            className="form-control"
                        />
                    </div>
                    <div className="d-grid">
                        <input type="submit" value="Login" onClick={() => setDataOnStorage(email)} className='btn btn-primary mt-3' />
                    </div>
                    <div className="mb-4">
                        <span>Not Yet Registered? </span>
                        <Link to='/Register'>
                            <span>Click here for Register</span>
                        </Link>
                    </div>

                </form>
            </div>
        </div>
    );
}

export default Login;