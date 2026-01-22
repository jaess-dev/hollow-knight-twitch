import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [data, setData] = useState<any>("no data");
    useEffect(() => {
        populateWeatherData()
    }, []);
    return <>
        {JSON.stringify(data)}
    </>

    async function populateWeatherData() {
        const response = await fetch('api');
        if (response.ok) {
            const data = await response.json();
            setData(data);
        }
    }
}




export default App;