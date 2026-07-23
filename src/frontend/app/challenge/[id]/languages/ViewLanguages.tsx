
'use client';
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { Manifest } from "@/lib/ClassTypes";
import { DeleteLanguageHandle } from "./actions";
import { useActionState } from "react";
import { usePathname } from "next/navigation";
import Link from "next/link";
export function ViewLanguages({id,manifests, own_id}:{id:string, own_id:string,manifests:Manifest[]}){
    if(!manifests){
        return <p>No manifests</p>
    }
    return(
        
        <div>
            {manifests.map((manifest:Manifest)=>(
                            <div>
                            <Link href={`languages/${manifest.languageId}/edit`} key={manifest.id}>{manifest.languageName}</Link>
                            {own_id === manifest.authorId.toString() && (
                                <DeleteLanguageForm 
                                    key={'d'+manifest.id} 
                                    id={id} 
                                    language_id={manifest.languageId.toString()}
                                /> 
                            )}
                            <br></br>
                            </div>
            ))}
            <br></br>
            <Link href={`/update_challenge/add_language/${id}`}>Add language support</Link>
        </div>
    )
}
export function DeleteLanguageForm({id, language_id}:{id: string, language_id:string}){
    const pathname = usePathname();
    const actionId=DeleteLanguageHandle.bind(null,id,language_id,pathname);
    const [state,formAction,isPending]=useActionState(actionId,null);
    
    return(
        <div>
            <form action={formAction}>
                <button type="submit">Delete</button>
            </form>
            
        </div>
    );
}