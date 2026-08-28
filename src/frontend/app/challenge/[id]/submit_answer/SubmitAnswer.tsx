'use client';

import { useState } from "react";
import { useActionState } from "react";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";
import Link from "next/link";
import { HandleAnswerPost } from "./actions";
import { Submission, Manifest} from "@/lib/ClassTypes";
import { LENGTHS } from "@/lib/globals";
export default function SubmitAnswer({challenge,manifests}:{challenge:Challenge,manifests:Manifest[]}){
    const HandleAnswerPostId=HandleAnswerPost.bind(null,challenge.id.toString());
    const[state,formAction,isPending]=useActionState(HandleAnswerPostId,null);
    const[selectedLanguage,setSelectedLanguage]=useState('');
    return(
        <div>
            <form action={formAction}>
            <p>Challenge #{challenge.id}: {challenge.title}</p>
            <select name="language" id="language" value={selectedLanguage} onChange={(e)=>setSelectedLanguage(e.target.value)}>
                    {(manifests).map((manifest:Manifest)=>(
                        
                        <option key={manifest.languageId} value={manifest.languageName}>{manifest.languageName}</option>
                    ))}
            </select>
            <textarea id="code" name="code" maxLength={LENGTHS.submission}></textarea>
            <button type="submit">Submit</button>
             </form>
        </div>
    );
}
