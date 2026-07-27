'use client';

import { useState } from "react";
import { useActionState } from "react";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";
import Link from "next/link";
import { HandleAnswerPost } from "./actions";
import { Manifest } from "@/lib/ClassTypes";
export function SubmitAnswer({challenge,manifests}:{challenge:Challenge,manifests:Manifest[]}){
    const[state,formAction,isPending]=useActionState(HandleAnswerPost,null);
    const[selectedLanguage,setSelectedLanguage]=useState('');
    return(
        <div>
            <form action={formAction}></form>
            <p>Challenge #{challenge.id}: {challenge.title}</p>
            <select name="language" id="languages" value={selectedLanguage} onChange={(e)=>setSelectedLanguage(e.target.value)}>
                    {(manifests).map((manifest:Manifest)=>(
                        <option value={manifest.languageName}>{manifest.languageName}</option>
                    ))}
            </select>
            <button type="submit">Submit</button>
             
        </div>
    );
}
