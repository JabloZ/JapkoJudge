
'use client';
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { Manifest } from "@/lib/ClassTypes";
import Link from "next/link";
export default function ViewLanguages({id,manifests}:{id:string, manifests:Manifest[]}){
    if(!manifests){
        return <p>No manifests</p>
    }
    return(
        
        <div>
            {manifests.map((manifest:Manifest)=>(
                            <Link href={`languages/${manifest.languageId}/edit`}>{manifest.languageName}</Link>
            ))}
        </div>
    )
}
