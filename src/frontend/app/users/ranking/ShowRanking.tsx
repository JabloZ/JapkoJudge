'use client';

import { useState } from "react";
import { Language, Rank } from "@/lib/ClassTypes";
import { GetRankingRequest } from "./actions";

export function ShowRanking({languages}: {languages: Language[]}){
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const [ranking, setRanking] = useState<Rank[]>([]);
    const [showRanking, setShowRanking] = useState(false);
    const [error, setError] = useState('');

    async function handleSearch() {
        if (!selectedLanguage) {
            setError('Wybierz język');
            return;
        }
        setError('');
        

        const res = await GetRankingRequest(Number(selectedLanguage));

        if (!res.success) {
            setError(res.error);
            
            return;
        }

        setRanking(res.ranking);
        setShowRanking(true);
        
    }

    return(
        <div>
            <label htmlFor="languages">Choose language</label>
            <select 
                name="language" 
                id="languages" 
                value={selectedLanguage} 
                onChange={(e) => setSelectedLanguage(e.target.value)}
            >
                <option value="">-- wybierz --</option>
                {languages.map((language: Language) => (
                    <option key={language.id} value={language.id}>{language.name}</option>
                ))}
            </select>
            <br/>
            <button onClick={handleSearch}>Search</button>

            {showRanking && ranking.map((rank: Rank) => (
                <p key={rank.username}>{rank.username} | Completed: {rank.challengeCount} | Score: {rank.score}</p>
            ))}
        </div>
    );
}