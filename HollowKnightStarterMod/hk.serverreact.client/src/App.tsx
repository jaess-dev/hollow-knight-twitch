import { Route, Routes } from 'react-router-dom';
import { CsOverlay } from './components/cs/CsOverlay';
import { HkOverlay } from './components/hk/HazardDeathEffect';
import ChatOverlay from './components/base/ChatOverlay';
import TheKnight from "@/assets/The knight.jpg"
import { Page } from './components/Page';
import Camera from './components/base/camera/Camera';
import GameOverlay from './components/base/GameOverlay';

function App() {
    return <>
        <Routes>
            <Route element={<Page />}>
                <Route path="/chat" element={<ChatOverlay backgroundImage={TheKnight} />} />
                <Route path="/game" element={<GameOverlay backgroundImage={TheKnight} />} />
                <Route path="/hk" element={<HkOverlay />} />
                <Route path="/cs" element={<CsOverlay />} />
            </Route>

            <Route path="/camera" element={<Camera />} />
        </Routes>
    </>
}



export default App;