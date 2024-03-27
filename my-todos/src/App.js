import React, { useEffect, useState } from "react";
import service from "./service.js";

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);

  const [title, setTitle] = useState();

  async function getTodos() {
    const todos = await service.getTasks();
    setTodos(todos);
  }

  async function getTitle() {
    const t = await service.getTitle();
    setTitle(t);
  }

  async function createTodo(e) {
    if (newTodo != "") {
      console.log("newtodo: " + newTodo);
      e.preventDefault();
      await service.addTask(newTodo);
      setNewTodo(""); //clear input
      await getTodos(); //refresh tasks list (in order to see the new one)
    }
  }

  async function updateCompleted(todo, isComplete) {
    await service.setCompleted(todo.id, isComplete);
    await getTodos(); //refresh tasks list (in order to see the updated one)
  }

  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos(); //refresh tasks list
  }

  useEffect(() => {
    console.log(process.env);
    getTodos();
    getTitle();
  }, []);

  return (
    <section className="todoapp">
      <header className="header">
        <h1>todos</h1>
        <form onSubmit={createTodo}>
          <input
            className="new-todo"
            placeholder="Well, let's take on the day"
            value={newTodo}
            onChange={(e) => setNewTodo(e.target.value)}
          />
          {/* <input type='submit' id="myBtn"></input> */}
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
          <h1>{title}</h1>
          {todos.map((todo) => {
            return (
              <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
                <div className="view">
                  <input
                    className="toggle"
                    type="checkbox"
                    defaultChecked={todo.isComplete}
                    onChange={(e) => updateCompleted(todo, e.target.checked)}
                  />
                  <label>{todo.name}</label>
                  <button
                    className="destroy"
                    onClick={() => deleteTodo(todo.id)}
                  ></button>
                </div>
              </li>
            );
          })}
        </ul>
      </section>
    </section>
  );
}

export default App;
