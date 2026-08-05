
'use client';
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { Manifest } from "@/lib/ClassTypes";
import { Submission } from "@/lib/ClassTypes";
import { useActionState } from "react";
import { usePathname } from "next/navigation";
import Link from "next/link";
export function ShowSubmissions({id,submissions, own_id}:{id:string, own_id:string,submissions:Submission[]}){
    if(!submissions){
        return <p>No manifests</p>
    }
    return(
        
        <div>
            {submissions.map((submission:Submission)=>(
                            <div>
                            <p>Submission ID:{submission.id}</p>
                            <Link href={`challenge/${submission.challengeId}`}>{submission.challengeTitle}</Link>
                            <p>Status: {submission.status}</p>  
                            <p>Memory used: {submission.memoryUsed}</p>
                            <p>Execution time: {submission.executionTime}</p>
                            <p>{submission.message}</p>
                            <br></br>
                            </div>
            ))}
            <br></br>
        </div>
    )//todo - return user an info (not just status)
    //todo #2 - make submission view, where user will get info about error etc.
}
