import axios from 'axios';
//axios.defaults.baseURL="https://localhost:5241";

const apiUrl = "http://localhost:5241"

//הוספת מיירט ללכידת שגיאות
axios.interceptors.request.use(function (config) {
  console.log("config: " + config);
  return config;
}, function (error) {
  console.log("error: " + error);
  return Promise.reject(error);
});

// Add a response interceptor
axios.interceptors.response.use(function (response) {
  // Any status code that lie within the range of 2xx cause this function to trigger
  console.log("response: " + response);
  return response;
}, function (error) {
  // Any status codes that falls outside the range of 2xx cause this function to trigger
  console.log("error: " + error);
  return Promise.reject(error);
});


export default {

  //שליפת המשימות
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/`)
    return result.data;
  },

  //הוספת משימה חדשה
  addTask: async (name) => {
    console.log("i am in the addTask");
    const result = await axios.post(`${apiUrl}/`, null, {
      params: {
        todo: name
      }
    });
    console.log('addTask', name)
    return result.data;
  },

  //עדכון סטטוס משימה
  setCompleted: async (id, isComplete) => {
    const result = await axios.put(`${apiUrl}/ToDoComplete/${id}`, null, {
      params: {
        complete: isComplete
      }
    });
    console.log('setCompleted', { id, isComplete });
    return result.data;
  },

  //מחיקת משימה
  deleteTask: async (id) => {
    const result = await axios.delete(`${apiUrl}/${id}`)
    console.log('deleteTask')
    return result.data;
  }
};
