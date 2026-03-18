import React, { useEffect, useState } from 'react';
import service from './service';

function TodoList() {
    const [newTodo, setNewTodo] = useState("");
    const [todos, setTodos] = useState([]);
    const [editingId, setEditingId] = useState(null); // שומר את ה-ID של המשימה שבעריכה
    const [editingText, setEditingText] = useState(""); // שומר את הטקסט הזמני בזמן שמקלידים

    const user = service.getCurrentUser();


    async function getTodos() {
        const tasks = await service.getTasks();
        setTodos(tasks);
    }

    useEffect(() => {
        getTodos();
    }, []);


    async function handleSaveEdit(id) {
        if (!editingText.trim()) {
            setEditingId(null);
            return;
        }
        await service.updateTaskName(id, editingText);
        setEditingId(null);
        getTodos();
    }

    return (
        <section className="todoapp">
            <header className="header">
                <button className="logout-btn" onClick={() => service.logout()}>התנתק</button>
                <h1>משימות של {user?.FullName || "אורח"}</h1>
                <form onSubmit={async (e) => {
                    e.preventDefault();
                    if (!newTodo.trim()) return;
                    await service.addTask(newTodo);
                    setNewTodo("");
                    getTodos();
                }}>
                    <input className="new-todo" placeholder="מה צריך לעשות?" value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
                </form>
            </header>
            <section className="main">
                <ul className="todo-list">
                    {todos.map(todo => (
                        <li className={`${todo.isComplete ? "completed" : ""} ${editingId === todo.id ? "editing" : ""}`} key={todo.id}>                            <div className="view">
                            <input className="toggle" type="checkbox" checked={todo.isComplete || false}
                                onChange={(e) => { service.setCompleted(todo.id, e.target.checked).then(getTodos) }} />

                            {/* לחיצה כפולה על הטקסט תכניס למצב עריכה */}
                            <label onDoubleClick={() => {
                                setEditingId(todo.id);
                                setEditingText(todo.name);
                            }}>
                                {todo.name}
                            </label>
                            <button className="destroy" onClick={() => service.deleteTask(todo.id).then(getTodos)}></button>
                        </div>

                            {/* אינפוט שמוצג רק כשאנחנו בעריכה */}
                            {editingId === todo.id && (
                                <input
                                    className="edit"
                                    value={editingText}
                                    onChange={(e) => setEditingText(e.target.value)}
                                    onBlur={() => handleSaveEdit(todo.id)} // שמירה כשיוצאים מהשדה
                                    onKeyDown={(e) => {
                                        if (e.key === 'Enter') handleSaveEdit(todo.id); // שמירה ב-Enter
                                        if (e.key === 'Escape') setEditingId(null); // ביטול ב-Escape
                                    }}
                                    autoFocus
                                />
                            )}
                        </li>
                    ))}
                </ul>
            </section>
        </section>
    );
}

export default TodoList;