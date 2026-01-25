import './App.css';
import { HkOverlay } from './components/HazardDeathEffect';

function App() {
    return <IsActive />
}

function IsActive() {
    // return <div>Status: {isConnected ? "Connected" : "Disconnected"}</div>;
    return <>
        <HkOverlay />
    </>
}

export default App;