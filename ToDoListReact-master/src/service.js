import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// 1. הגדרת כתובת ה-API שלך (תבדקי אם הפורט 5163 נכון אצלך!)
// const apiUrl = " https://todo-list-server-wesl.onrender.com";
const apiUrl = process.env.REACT_APP_API_URL;
axios.defaults.baseURL = apiUrl;

axios.defaults.headers.post['Content-Type'] = 'application/json';

// הוספת הטוקן לכל בקשה שיוצאת (Request Interceptor)
axios.interceptors.request.use(config => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`; // בדיוק כמו שעשינו ב-Swagger!
    }
    return config;
});

// 2. Interceptor לתפיסת שגיאות ורישום ללוג (כפי שנתבקשת במטלה)
axios.interceptors.response.use(
    (response) => {
        // אם הקריאה הצליחה, פשוט מחזירים את התשובה כפי שהיא
        return response;
    },
    (error) => {
        // אם הייתה שגיאה (למשל: השרת נפל, או ID לא נמצא)
        // אנחנו רושמים אותה ללוג בצורה מפורטת
        console.error("Axios Global Interceptor caught an error:", {
            status: error.response?.status,    // קוד השגיאה (למשל 404)
            data: error.response?.data,        // ההודעה מה-API (למשל "Item not found")
            message: error.message             // הודעת השגיאה הכללית של הדפדפן
        });

        // חשוב: מחזירים Promise.reject כדי שהקוד שקרא לפונקציה ידע שהייתה שגיאה
        return Promise.reject(error);
    }
);



export default {
    // שליפת כל המשימות
    getTasks: async () => {
        const result = await axios.get("/items");
        return result.data;
    },

    // הוספת משימה חדשה
    addTask: async (name) => {
        // אנחנו שולחים אובייקט עם שם, ה-API יגדיר ID וסטטוס
        const result = await axios.post("/items", { name: name, isComplete: false });
        return result.data;
    },

    // עדכון סטטוס השלמה
    setCompleted: async (id, isComplete) => {
        // שולחים את ה-ID בנתיב ואת הסטטוס בגוף הבקשה
        const result = await axios.put(`/items/${id}`, { isComplete: isComplete });
        return result.data;
    },
    updateTaskName: async (id, name) => {
        const result = await axios.put(`/items/${id}`, { name: name });
        return result.data;
    },

    // מחיקת משימה
    deleteTask: async (id) => {
        const result = await axios.delete(`/items/${id}`);
        return result.data;
    },

    login: async (email, password) => {
        const result = await axios.post("/login", { email, password });
        localStorage.setItem("token", result.data.token); // שמירת הטוקן
        return result.data.user;
    },
    register: async (fullName, email, password) => {
        const result = await axios.post("/register", { fullName, email, password });
        localStorage.setItem("token", result.data.token);
        return result.data.user;
    },
    logout: () => {
        console.log("Logging out user...");
        localStorage.removeItem("token");
        window.location.replace("/login");
    },
    getCurrentUser: () => {
        const token = localStorage.getItem("token");
        if (!token) return null;
        try {
            return jwtDecode(token);
        } catch (e) {
            return null;
        }
    },

};