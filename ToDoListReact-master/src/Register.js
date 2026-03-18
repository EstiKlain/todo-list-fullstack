import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import service from './service';

function Register() {
    const [formData, setFormData] = useState({ fullName: "", email: "", password: "" });
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await service.register(formData.fullName, formData.email, formData.password);
            navigate('/'); // אחרי הרשמה עוברים למשימות
        } catch (err) {
            alert("ההרשמה נכשלה. ייתכן שהמשתמש כבר קיים.");
        }
    };

    return (
        <div className="auth-container">
            <h2>יצירת חשבון</h2>
            <form onSubmit={handleSubmit}>
                <input type="text" placeholder="שם מלא" onChange={e => setFormData({...formData, fullName: e.target.value})} required />
                <input type="email" placeholder="אימייל" onChange={e => setFormData({...formData, email: e.target.value})} required />
                <input type="password" placeholder="סיסמה" onChange={e => setFormData({...formData, password: e.target.value})} required />
                <button type="submit">הירשם</button>
            </form>
            <p>כבר יש לך חשבון? <Link to="/login">היכנס כאן</Link></p>
        </div>
    );
}

export default Register;