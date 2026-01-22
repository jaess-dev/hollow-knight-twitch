import './App.css';
import { useSignalR } from './lib/signalr/useSignalR';

function App() {
    return <IsActive />
}

function IsActive() {
    const { isConnected } = useSignalR();
    return <div>Status: {isConnected ? "Connected" : "Disconnected"}</div>;
}

export default App;