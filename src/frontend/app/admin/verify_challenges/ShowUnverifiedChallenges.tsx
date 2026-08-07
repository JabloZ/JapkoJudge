'use client';

import { useState } from "react";
import { useActionState } from "react";
import { GetChallengesRequest, PostChallengeDecision, PostChallengeDifficulty } from "./actions";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";

export function ShowUnverifiedChallenges({challenges}:{challenges:Challenge[]}){
    const [selectedDifficulty, setSelectedDifficulty]=useState('');
    
    return(
        <div>
            {challenges.map((challenge: Challenge)=>(
                 <div key={challenge.id}>
                    {challenge.verified ? (
                    <form action={async () => { await PostChallengeDecision(challenge.id.toString(), false); }}>
                        <button type="submit">Unverify</button>
                    </form>
                    ) : (
                    <form action={async () => { await PostChallengeDecision(challenge.id.toString(), true); }}>
                    <button type="submit">Verify</button>
                    </form>
                    )}
                    <label htmlFor="languages">Choose difficulty</label>
                    <form action={async ()=>{await PostChallengeDifficulty(challenge.id.toString(), selectedDifficulty);}}>
                        <select name="difficulty" id="difficulty" value={selectedDifficulty} onChange={(e)=>setSelectedDifficulty(e.target.value)}>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                        </select>
                        <button type="submit">Change Difficulty</button>
                    </form>
                    <p>Difficulty: {challenge.difficulty}</p>
                    <ChallengeCard challenge={challenge}/>
                </div>
            ))}
        </div>
    );
}
