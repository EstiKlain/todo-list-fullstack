import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import service from './service';

function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await service.login(email, password);
            window.location.href = "/"; // ריענון קטן כדי לעדכן את ה-State של האפליקציה
        } catch (err) {
            alert("פרטים שגויים");
        }
    };

    return (
        <div className="auth-container">
            <h2>כניסה</h2>
            <form onSubmit={handleSubmit}>
                <input type="email" placeholder="אימייל" onChange={e => setEmail(e.target.value)} required />
                <input type="password" placeholder="סיסמה" onChange={e => setPassword(e.target.value)} required />
                <button type="submit">התחבר</button>
            </form>
            <p>אין לך חשבון? <Link to="/register">הירשם עכשיו</Link></p>
        </div>
    );
}

export default Login;