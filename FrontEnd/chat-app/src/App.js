import { Route, Routes } from 'react-router-dom';
import './App.css';
import Chat from './Chat';
import Login from './Login';
import Register from './Register';


function App() {
  return (
    <div className='container' style={{ padding: 20 }}>
      <Routes>
        <Route exact path='/register' element={<Register/>}></Route>
        <Route exact path='/' element={<Login />}></Route>
        <Route exact path='/chat' element={<Chat />}></Route>
      </Routes>
    </div>
  );
}

export default App;
