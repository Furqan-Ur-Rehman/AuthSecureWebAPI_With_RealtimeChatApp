import React from 'react';
import { useEffect, useState, useRef, Fragment } from 'react';
import * as signalR from '@microsoft/signalr';
import './App.css';

const baseURL = 'https://localhost:7246';
function Chat() {
    const [connection, setConnection] = useState(null);
    const [messages, setMessages] = useState([]);
    const [text, setText] = useState('');
    const [senderemail, setSenderEmail] = useState('');
    const [receiveremail, setReceiverEmail] = useState('');
    const messagesRef = useRef([]);

    useEffect(() => {
        setSenderEmail(localStorage.getItem('chatEmail'))
    }, [])

    //This below useEffect and messagesRef used for scroll down automatically when new message received.
    useEffect(
        () => {
            if (messagesRef.current) {

                messagesRef.current.scrollIntoView({ behavior: "smooth" });
            }
        }, [messages]
    );

    useEffect(() => {
        if (!senderemail) return; // wait until senderemail is set
        // create connection
        const conn = new signalR.HubConnectionBuilder()
            .withUrl(`${baseURL}/chat?email=${senderemail}`, { accessTokenFactory: () => localStorage.getItem("jwt") })
            .withAutomaticReconnect()
            .build();

        conn.start()
            .then(() => console.log('Connected to SignalR', senderemail))
            .catch(e => console.error('SignalR connection error:', e));

        conn.on('ReceiveMessage', (chats, senderEmail, receiverEmail, datetime) => {
            // chats, senderEmail, datetime are ChatMessages object from server
            // setMessages([
            //     chats, receiveEmail
            // ]); wrong method due to only pass array instead of array of object like below:
            setMessages(prev => [...prev,
            {
                text: chats,
                sender: senderEmail,
                receiver: receiverEmail,
                dateTime: datetime
            }
            ]);
            // console.log(msg)
        });

        setConnection(conn);

        return () => {
            conn.stop();
        };
    }, [senderemail]);

    // useEffect(() => {
    //     // fetch history on mount
    //     axios(`${baseURL}/api/chat/history`)
    //         .then(r => r.json())
    //         .then(data => setMessages(data))
    //         .catch(console.error);
    // }, []);

    const send = async (e) => {
        e.preventDefault();
        if (!connection) return;
        if (!text.trim()) return;
        try {
            if(senderemail == null)
                alert("You are not Logged in, Please Logged in First, then Start Chat!")
            // call server hub method
            await connection.invoke('SendMessage', senderemail, receiveremail, text || null);
            // let emailFromLocalStorage = localStorage.getItem('chatEmail');
            // if(senderemail !== emailFromLocalStorage){
            //     alert('Your Sender Email is In Correct, Please Write Correct Email!')
            // }
            setText("")
        } catch (err) {
            console.error(err);
        }
    };

    
    return (
        <div>
            <h2 className='App' style={{ fontSize: '30px', color: 'white', fontFamily: 'fantasy' }}>CHAT APP</h2>

            <form onSubmit={send}>
                <div className='d-flex' style={{ marginBottom: 12 }}>
                    <input type='email' disabled title='This field is disabled for any changes!' required className='form-control' style={{ width: '50%', fontFamily: 'fantasy' }} value={senderemail} onChange={e => setSenderEmail(e.target.value)} placeholder="Sender Email..." />
                    <input type='email' required className='form-control' style={{ width: '50%', marginLeft: 8, fontFamily: 'fantasy' }} value={receiveremail} onChange={e => setReceiverEmail(e.target.value)} placeholder="Receiver Email..." />
                </div>

                <div className='form-control msg' style={{ height: 400, fontSize: '20px', padding: 8, marginBottom: 8, background: 'white', fontFamily: 'monospace' }}>
                    {
                        messages.map((m) => {
                            return (
                                <Fragment key={m.id ?? Math.random()}>
                                    <div className={`message ${m.sender === senderemail ? "sent" : "received" } input_msg`}  ref={m.id === messages.length - 1 ? messagesRef : null}>
                                        <div style={{ fontSize: 17, color: '#555' }}>
                                            <strong>From:{m.sender}</strong>
                                        </div>
                                        <strong>{m.text}</strong>
                                        <p style={{ fontSize: 13, fontFamily: 'sans-serif' }}>{new Date(m.dateTime).toLocaleString()}</p>
                                    </div>
                                </Fragment>
                            )
                        })
                    }
                    <div ref={messagesRef} />
                    {/* {messages} */}
                </div>

                <div className='d-flex'>
                    <input type='text' required className='form-control' value={text} onChange={e => setText(e.target.value)} title='Type Message here' autoFocus placeholder="Type message here..." style={{ width: '98%', height: '50px', fontSize: '19px', fontFamily: 'fantasy' }} />
                    <input className='btn btn-dark' type='submit' value='Send' style={{ marginLeft: 8, height: '50px', width: '80px', cursor: 'pointer', fontSize: '19px', fontFamily: 'fantasy' }} />
                </div>
            </form>
        </div>
    )
}


export default Chat
